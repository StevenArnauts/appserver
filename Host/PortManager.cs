using System;
using System.Collections.Generic;
using System.Linq;
using Utilities;

namespace Core.ProcessHost {

	internal class PortManager {

		private readonly List<int> _ports = new List<int>();
		private readonly int _lower;
		private readonly int _upper;

		public PortManager(int lower, int upper) {
			this._upper = upper;
			this._lower = lower;
		}

		public int ClaimNext() {
			lock (this._ports) {
				if(!this._ports.Any()) {
					this._ports.Add(IPHelper.FindFreePort(this._lower, this._upper));
					if(this._ports[0] < 0) throw new Exception("Unable to find a free port between " + this._lower + " and " + this._upper);
					return (this._ports[0]);
				} 
				int nextPort = this._lower;
				while(nextPort < this._upper) {
					if (!this._ports.Contains(nextPort) && IPHelper.IsPortFree(nextPort)) {
						this._ports.Add(nextPort);
						return (nextPort);
					}
					nextPort = nextPort + 1;
				}
				throw new Exception("Unable to find a free port between 8500 and 9000");
			}
		}

		public void ReleasePort(int port) {
			lock (this._ports) {
				this._ports.Remove(port);
			}
		}

	}

}
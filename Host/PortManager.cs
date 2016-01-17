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

		public int GetNextFreePort() {
			lock (this._ports) {
				if(!this._ports.Any()) {
					this._ports.Add(IPHelper.FindFreePort(this._lower, this._upper));
					if(this._ports[0] < 0) throw new Exception("Unable to find a free port between " + this._lower + " and " + this._upper);
					return (this._ports[0]);
				} 
				int nextPort = this._ports.Last();
				while(nextPort < this._upper) {
					nextPort = nextPort + 1;
					if (IPHelper.IsPortFree(nextPort)) {
						this._ports.Add(nextPort);
						return (nextPort);
					}
				}
				throw new Exception("Unable to find a free port between 8500 and 9000");
			}
		}

	}

}
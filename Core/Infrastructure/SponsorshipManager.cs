using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Lifetime;
using Utilities;

namespace Core {

	public class SponsorshipManager : MarshalByRefObject, ISponsor {

		private readonly object _lock = new object();
		private readonly List<ILease> _leaseList = new List<ILease>();

		TimeSpan ISponsor.Renewal(ILease lease) {
			Debug.Assert(lease.CurrentState == LeaseState.Active);
			Logger.Debug(this, "Extended lease #" + lease.GetHashCode() + " with " + lease.InitialLeaseTime);
			return lease.InitialLeaseTime;
		}

		//~SponsorshipManager() {
		//	this.UnregisterAll();
		//}

		public void OnExit(object sender, EventArgs e) {
			this.UnregisterAll();
		}

		public void Register(MarshalByRefObject obj) {
			if(RemotingServices.IsTransparentProxy(obj)) {
				ILease lease = (ILease)RemotingServices.GetLifetimeService(obj);
				Debug.Assert(lease.CurrentState == LeaseState.Active);
				lease.Register(this);
				lock(this._lock) {
					this._leaseList.Add(lease);
					Logger.Debug(this, "Sponsoring lease #" + lease.GetHashCode() + " for proxy to " + obj.GetType().Name + ", id = #" + obj.GetHashCode() + ", url = " + RemotingServices.GetObjectUri(obj));
				}
			}
		}

		public void UnregisterAll() {
			lock(this._lock) {
				int index = 0;
				while(this._leaseList.Count > 0) {
					ILease lease = this._leaseList[index];
					lease.Unregister(this);
					this._leaseList.RemoveAt(index);
					Logger.Debug(this, "Removevd lease #" + lease.GetHashCode());
					index++;
				}
			}
		}

		public void Unregister(MarshalByRefObject obj) {
			ILease lease = (ILease)RemotingServices.GetLifetimeService(obj);
			Debug.Assert(lease.CurrentState == LeaseState.Active);
			lease.Unregister(this);
			lock(this._lock) {
				this._leaseList.Remove(lease);
				Logger.Debug(this, "Stopped sponsoring lease #" + lease.GetHashCode() + " for proxy to " + obj.GetType().Name + ", id = #" + obj.GetHashCode() + ", url = " + RemotingServices.GetObjectUri(obj));
			}
		}

	}

}
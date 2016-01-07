using System;
using System.Runtime.Remoting.Lifetime;
using Utilities;

namespace Core {

	public abstract class RemoteObject : MarshalByRefObject, IDisposable {

		protected RemoteObject() {
			Logger.Debug(this, this.GetType().FullName + "#" + this.GetHashCode() + " created in app domain " + AppDomain.CurrentDomain.FriendlyName);
		}

		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public override object InitializeLifetimeService() {
			ILease lease = (ILease)base.InitializeLifetimeService();
			if(lease == null) {
				Logger.Warn(this, "No lease");
				return null;
			}
			lease.InitialLeaseTime = TimeSpan.FromSeconds(30);
			lease.RenewOnCallTime = TimeSpan.FromSeconds(0);
			lease.SponsorshipTimeout = TimeSpan.FromSeconds(5);
			Logger.Info(this, "Set lease times on " + this.GetHashCode());
			return lease;
		}

		protected virtual void Dispose(bool disposing) {
			if(disposing) { }
			Logger.Warn(this, "Disposed");
		}

		~RemoteObject() {
			this.Dispose(false);
		}

	}

}
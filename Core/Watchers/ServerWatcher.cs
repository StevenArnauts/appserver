using System;
using System.Threading;
using Utilities;

namespace Core {

	public class ServerWatcher : Watcher {

		private readonly Proxy _proxy;
		private Thread _worker;

		public ServerWatcher(Uri uri, Application application) : base(application) {
			this._proxy = new Proxy(uri);
			Logger.Info(this, "Using URL " + uri.AbsoluteUri + " for application " + application.Name);
		}

		public void Start() {
			Logger.Info(this, "Starting...");
			this._worker = new Thread(this.Poll) { IsBackground = true };
			this._worker.Start();
		}

		public void Stop() {
			Logger.Info(this, "Stopping...");
			if (this._worker != null && this._worker.IsAlive) {
				this._worker.Abort();
			} else {
				Logger.Warn(this, "Worker will not be aborted because it is " + (this._worker == null ? "null" : this._worker.ThreadState.ToString()));
			}
		}

		private void Poll() {
			Logger.Info(this, "Started");
			while (true) {
				Pauze();
				try {
					Update newVersion = this._proxy.Check(this.Application.Name, this.Application.CurrentVersion);
					if (newVersion != null) {
						Logger.Info(this, "Server has a new version " + newVersion.Number + " for " + newVersion.Application);
						byte[] bytes = this._proxy.Download(newVersion);
						Package newPackage = BytePackage.Create(bytes, Proxy.DownloadUrl(newVersion));
						this.Update(newPackage);
					}
				} catch (ThreadAbortException) {
					Logger.Info(this, "Aborted");
					break;
				} catch (Exception ex) {
					Logger.Error("Polling failed: " + ex.GetRootCause().Message);
					Pauze();
				} 
			}
			Logger.Info(this, "Stopped");
		}

		private static void Pauze() {
			Thread.Sleep(TimeSpan.FromSeconds(10));
		}

		public void Reset() {
			this.Stop();
			this.Start();
		}

	}

}
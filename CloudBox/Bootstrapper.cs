using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Core.Contract;
using Utilities;

namespace Cloudbox {

	public class Bootstrapper : IBootstrapper {

		private Context _context;

		public void Initialize(Context context) {
			Logger.Initialize("log4net.config");
			this._context = context;
			Logger.Info(this, "Initialized in app domain " + AppDomain.CurrentDomain.FriendlyName + ", my version is " + this.GetType().Assembly.GetName().Version);
		}

		public void Run(CancellationToken token) {
			Logger.Info(this, "Started, SomeConnectorSetting = " + ConfigurationManager.AppSettings.Get("SomeConnectorSetting"));
			int counter = 1;
			while (true) {
				try {
					if (this._context != null) this._context.HandleMessage(new Message { Id = "nr." + counter, Content = "message from cloudbox" });
					counter++;
					Task.Delay(TimeSpan.FromSeconds(10), token).Wait(token);
				} catch (OperationCanceledException) {
					Logger.Info(this, "Stopped");
					break;
				} catch (Exception ex) {
					Logger.Error(ex);
				}
			}
			Logger.Info(this, "Stopped");
		}

		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing) {
			if (disposing) {}
			Logger.Warn(this, "Disposed");
		}

		~Bootstrapper() {
			this.Dispose(false);
		}

	}

}
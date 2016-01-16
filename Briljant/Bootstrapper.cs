using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Core.Contract;
using Utilities;

namespace Briljant {

	public class Bootstrapper : IBootstrapper {

		private ServerContext _context;

		public void Initialize(ServerContext context) {
			Logger.Initialize("log4net.config");
			this._context = context;
			Logger.Info(this, "Initialized in app domain " + AppDomain.CurrentDomain.FriendlyName + ", my version is " + this.GetType().Assembly.GetName().Version);
		}

		public void Run(CancellationToken token) {
			Logger.Info(this, "Starting, SomeConnectorSetting = " + ConfigurationManager.AppSettings.Get("SomeConnectorSetting"));
			try {
				int counter = 1;
				while(true) {
					if(this._context != null) this._context.HandleMessage(new Message { Id = "nr." + counter, Content = "message from briljant" });
					counter++;
					Task.Delay(TimeSpan.FromSeconds(10), token).Wait(token);
				}
			} catch(OperationCanceledException) {
				Logger.Info(this, "Cancelled");
			} catch(Exception ex) {
				Logger.Error(ex);
			}
			Logger.Info(this, "Stopped");
		}

		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing) {
			if(disposing) { }
			Logger.Warn(this, "Disposed");
		}

		~Bootstrapper() {
			this.Dispose(false);
		}

	}

}
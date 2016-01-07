using System;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Core.Contract;
using Utilities;

namespace Alure {

	public class Bootstrapper : IBootstrapper {

		private Context _context;

		public Bootstrapper() {
			Logger.Initialize("log4net.config");
			Logger.Info(this, "Created");
		}

		public void Initialize(Context context) {
			this._context = context;
			Logger.Info(this, "Initialized in app domain " + AppDomain.CurrentDomain.FriendlyName + ", my version is " + this.GetType().Assembly.GetName().Version);
		}

		public void Run(CancellationToken token) {
			Logger.Info(this, "Started, SomeConnectorSetting = " + ConfigurationManager.AppSettings.Get("SomeConnectorSetting"));
			string settingsPath = this._context.ConfigFilePath;
			Logger.Debug(this, "Loading settings from " + settingsPath + "...");
			using (FileStream stream = File.Open(settingsPath, FileMode.Open, FileAccess.Read)) {
				UserSettings settings = XmlSerializer.Deserialize<UserSettings>(stream);
				Logger.Info(this, "Repository location = " + settings.Repository.Location);
			}
			int counter = 1;
			while (true) {
				try {
					if (this._context != null) this._context.HandleMessage(new Message { Id = "nr." + counter, Content = "message from alure" });
					counter++;
					Task.Delay(TimeSpan.FromSeconds(10), token).Wait(token);
				} catch (OperationCanceledException) {
					Logger.Info(this, "Cancelled");
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
using Utilities;

namespace Core {

	public abstract class Watcher {

		private readonly Application _application;

		protected Watcher(Application application) {
			this._application = application;
		}

		protected Application Application {
			get { return (this._application); }
		}

		protected void Update(Package package) {
			Logger.Info(this, "Updating " + this._application.Name + "...");
			this._application.Stop().Wait();
			this._application.Unload().Wait();
			this._application.Deploy(package).Wait();
			this._application.Start().Wait();
			Logger.Info(this, "Update complete");
		}

	}

}
using System.IO;
using Utilities;

namespace Core {

	/// <summary>
	/// Whatches the original package (.zip) and executes the action when the archive is modified.
	/// Locks files...
	/// </summary>
	public class PackageWatcher : Watcher {

		private DirectoryMonitor _watcher;
		private readonly string _folder;
		private readonly string _filter;

		public PackageWatcher(string folder, string filter, Application application) : base(application) {
			this._filter = filter;
			this._folder = folder;
			Logger.Info(this, "Watching folder " + Path.GetFullPath(folder) + " using filter " + filter + " for updates for application " + application.Name);
		}

		public void Start() {
			this._watcher = new DirectoryMonitor(this._folder, this._filter);
			this._watcher.Change += this.WatcherOnChange;
			this._watcher.Start();
		}

		private void WatcherOnChange(string path) {
			Logger.Info(this, "Detected that package " + path + " was changed");
			Package newPackage = FilePackage.Open(path);
			this.Update(newPackage);
		}

		public void Stop() {
			this._watcher.Stop();
		}

		public void Reset() {
			this.Stop();
			this.Start();
		}

	}

}
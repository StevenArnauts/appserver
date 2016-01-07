using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Contract;
using Utilities;

namespace Core {

	/// <summary>
	/// Manages one or more applications in their own <seealso cref="AppDomain"/>.
	/// Applications are bundled in a <see cref="Package"/>, like a .jar file in Java.
	/// </summary>
	public class Server : MarshalByRefObject, IDisposable {

		private const string DEFAULT_APP_FOLDER = @".\apps";

		private readonly List<Application> _applications = new List<Application>();
		private readonly string _appFolder;
		private readonly string _tempFolder;

		private bool _disposed;
		private readonly SameThreadTaskScheduler _scheduler;

		public static Server Create(string appFolder = null, string tempFolder = null) {
			string root = Path.GetFullPath(appFolder ?? DEFAULT_APP_FOLDER);
			if (!Directory.Exists(root)) Directory.CreateDirectory(root);
			string temp = Path.GetFullPath(tempFolder ?? Path.Combine(Path.GetTempPath(), "Kluwer", "Install"));
			Server server = new Server(root, temp);
			return (server);
		}

		public string AppFolder {
			get { return (this._appFolder); }
		}

		public string TempFolder {
			get { return ( this._tempFolder ); }
		}

		private Server(string appFolder, string tempFolder) {
			this._appFolder = appFolder;
			this._tempFolder = tempFolder;
			this._scheduler = new SameThreadTaskScheduler("AppServer");
			Logger.Info(this, "Created, app folder = " + appFolder + ", temp folder = " + tempFolder);
		}

		public Task Run(Action action) {
			Task task = new Task(action);
			task.Start(this._scheduler);
			return ( task );
		}

		public Application CreateApplication(string name) {
			if(this._applications.Any(a => a.Name == name)) throw new Exception("Application '" + name + "' is already registered");
			Application application = new Application(name, this);
			application.Init(new Context(application, this));
			this._applications.Add(application);
			Logger.Info(this, "Registered application " + name);
			return (application);
		}

		public void Stop() {
			Logger.Info(this, "Stopping...");
			if (this._applications.Any()) {
				foreach(Application application in this._applications) {
					Logger.Info(this, "Signalling application " + application.Name + " to stop...");
					application.Stop().Wait();
					application.Unload().Wait();
				}
				this._applications.Clear();	
			}
			Logger.Info(this, "Stopped");
		}
		
		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing) {
			if(this._disposed) return;
			if(disposing) {
				this.Stop();
				this._scheduler.Dispose();
			}
			// Free any unmanaged objects here.
			this._disposed = true;
		}

	}

}
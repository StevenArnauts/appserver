using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Contract;
using Core.Persistence;
using Utilities;

namespace Core {

	/// <summary>
	/// Manages one or more applications in their own <seealso cref="AppDomain"/>.
	/// Applications are bundled in a <see cref="Package"/>, much like a .jar file in Java.
	/// </summary>
	public class Server : MarshalByRefObject, IDisposable {

		private const string DEFAULT_APP_FOLDER = @".\apps";

		private readonly List<Application> _applications = new List<Application>();
		private readonly SameThreadTaskScheduler _scheduler;
		private readonly IApplicationRepository _repository;
		private bool _disposed;

		public static Server Create(string appFolder = null, string tempFolder = null) {
			string root = Path.GetFullPath(appFolder ?? DEFAULT_APP_FOLDER);
			string temp = Path.GetFullPath(tempFolder ?? Path.Combine(Path.GetTempPath(), "Kluwer", "Install"));
			Server server = new Server(root, temp);
			return (server);
		}

		/// <summary>
		/// Loads the last deployed version of all applications.
		/// </summary>
		public IEnumerable<Application> Load() {
			List<Application> applications = new List<Application>();
			foreach (FileSystemApplication application in this._repository.GetApplications()) {
				Logger.Info(this, "Loading existing application " + application.Name + "...");
				List<FileSystemPackage> packages = this._repository.GetPackages(application).ToList();
				if (!packages.Any()) {
					Logger.Info(this, "Application " + application.Name + " does not have any packages deployed");
					continue;
				}
				FileSystemPackage latest = packages.Last();
				Logger.Info(this, "Application " + application.Name + " latest package = " + latest.Version.ToString(4));
				Application app = this.CreateApplication(application.Name);
				app.LoadFrom(latest.Directory);
				app.Init(new Context(app, this));
				applications.Add(app);
			}
			return (applications);
		}

		/// <summary>
		/// Gets the folder where applications are deployed
		/// </summary>
		public string RootFolder {
			get { return (this._repository.RootFolder); }
		}

		public string TempFolder {
			get { return ( this._repository.TempFolder ); }
		}

		private Server(string appFolder, string tempFolder) {
			this._repository = new FileSystemRepository(new FileSystemRepositoryConfiguration(appFolder, tempFolder));
			this._scheduler = new SameThreadTaskScheduler("AppServer");
			Logger.Info(this, "Created, app folder = " + appFolder + ", temp folder = " + tempFolder);
		}

		/// <summary>
		/// Queues and runs the action on the same thread this server was created on.
		/// </summary>
		/// <param name="action"></param>
		/// <returns></returns>
		public Task Run(Action action) {
			Task task = new Task(action);
			task.Start(this._scheduler);
			return ( task );
		}

		public Application CreateApplication(string name) {
			if(this._applications.Any(a => a.Name == name)) throw new Exception("Application '" + name + "' is already registered");
			Application application = new Application(name, this, this._repository);
			application.Init(new Context(application, this));
			this._applications.Add(application);
			Logger.Info(this, "Registered application " + name);
			return (application);
		}

		public bool TryGetApplication(string name, out Application application) {
			application = this._applications.FirstOrDefault(a => a.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
			return (application != null);
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
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Contract;
using Utilities;

namespace Core {

	/// <summary>
	/// The "logical" application
	/// </summary>
	public class Application : MarshalByRefObject {

		private readonly string _name;
		private readonly object _lock = new object();
		
		private Context _context;

		private AppDomain _appDomain;
		private Runner _runner;
		private DeploymentInfo _deployment;
		private readonly Server _server;

		internal Application(string name, Server server) {
			this._server = server;
			this._name = name;
		}

		internal void Init(Context context) {
			this._context = context;
		}

		/// <summary>
		/// The name of the application
		/// </summary>
		public string Name {
			get { return this._name; }
		}

		public string SettingsPath {
			get { return (this._deployment.SettingsPath); }
		}

		public string BinFolder {
			get { return (this._deployment.BinFolder); }
		}

		public string CurrentVersion {
			get { return this._deployment == null ? null : this._deployment.PackageInfo.Bootstrapper.Assembly.Version; }
		}

		public Task Deploy(Package package) {
			return (this._server.Run(() => {
				lock(this._lock) {
					Logger.Info(this, "Deploying package from " + package.Source + "...");
					this._deployment = this.PrepareDeployment(package);
					this._deployment.BinFolder = Path.Combine(this._context.AppFolder, this.Name, "bin", this._deployment.PackageInfo.Bootstrapper.Assembly.Version);
					this._deployment.SettingsFolder = Path.Combine(this._context.AppFolder, this.Name, "conf");
					package.Deploy(this._deployment);
					this.Load();
					foreach(Contract.Type updaterInfo in this._deployment.PackageInfo.Updaters) {
						try {
							Updater updater = this.CreateInstance<Updater>(this._appDomain, updaterInfo);
							updater.Run(this._context);
						} catch(Exception ex) {
							Logger.Error("Failed to run updater " + updaterInfo.FullName + " because: " + ex.GetRootCause().Message);
						}
					}
					Logger.Info(this, "Package deployed");
				}
			}));
		}

		public Task Unload() {
			return (this._server.Run(() => {
				int attempt = 0;
				const int ATTEMPTS = 10;
				if (this._appDomain == null) {
					Logger.Warn(this, "Appdomain is null");
					return;
				}
				while (attempt <= ATTEMPTS) {
					try {
						attempt++;
						Logger.Info(this, "Unloading, attempt " + attempt + " of " + ATTEMPTS + "...");
						AppDomain.Unload(this._appDomain);
						break;
					} catch (CannotUnloadAppDomainException) {
						Logger.Warn(this, "Failed to stop in time because the appdomain could not be unloaded");
					} catch (Exception e) {
						Logger.Error(this, e, "Failed to stop.");
						break;
					}
				}
				this._appDomain = null;
				this._runner = null;
				Logger.Info(this, "Unloaded");
			}));
		}

		public Task Start() {
			return (this._server.Run(() => {
				lock (this._lock) {
					Logger.Info(this, "Starting...");
					if (this._runner == null) throw new Exception("Bootstrapper not set");
					this._runner.Initialize(this._context, this._deployment.PackageInfo.Bootstrapper);
					this._runner.Start();
					Logger.Info(this, "Started");
				}
			}));
		}

		public Task Stop() {
			return (this._server.Run(() => {
				lock (this._lock) {
					Logger.Info(this, "Stopping...");
					try {
						if (this._runner == null) {
							Logger.Warn(this, "Application " + this._name + " cannot be safely stopped because the connection to the runner is not set");
							return;
						}
						this._runner.Stop();
						Logger.Info(this, "Stopped");
					} catch (Exception ex) {
						Logger.Error(this, ex, "Stopping application " + this._name + " failed");
					}
				}
			}));
		}

		private void Load() {
			Logger.Info(this, "Loading...");
			AppDomain appDomain = Utilities.AppDomainManager.LoadAppDomain(this._deployment.BinFolder, this._deployment.PackageInfo.Bootstrapper.Assembly.File);
			this._runner = this.CreateInstance<Runner>(appDomain, Contract.Type.FromType(typeof(Runner)));
			this._appDomain = appDomain;
			Logger.Info(this, "Loaded");
		}

		private DeploymentInfo PrepareDeployment(Package package) {
			DeploymentInfo info = new DeploymentInfo { PackageInfo = package.ExtractPackageInfo(this._context.GetTempPath()) };
			return ( info );
		}

		private TInstance CreateInstance<TInstance>(AppDomain appDomain, Contract.Type type) where TInstance : class {
			string file = type.Assembly.File;
			if(file == null) throw new ArgumentException("Type must include assembly location");
			object proxy = RemoteObjectFactory.Create(appDomain, type, null);
			TInstance instance = proxy as TInstance;
			if(instance == null) throw new Exception("Type " + type.FullName + " cannot be cast to " + typeof(TInstance).FullName);
			Logger.Info(this, "Created instance of " + type.FullName + " from " + file + " in app domain " + appDomain.FriendlyName);
			return (instance);
		}
	
	}

}
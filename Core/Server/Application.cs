﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Core.Contract;
using Core.Infrastructure;
using Core.Persistence;
using Utilities;

namespace Core {

	/// <summary>
	/// The "logical" application
	/// </summary>
	public class Application : MarshalByRefObject {

		private readonly object _lock = new object();
		private readonly string _name;
		private readonly Server _server;
		private readonly IApplicationRepository _packageRepository;
		private ServerContext _context;
		private Deployment _deployment;
		private readonly IHostingModel _hostingModel;
		private IApplicationHost _applicationHost;

		internal Application(string name, Server server, IApplicationRepository packageRepository, IHostingModel hostingModel) {
			this._hostingModel = hostingModel;
			this._packageRepository = packageRepository;
			this._server = server;
			this._name = name;
		}

		internal void Init(ServerContext context) {
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
			get { return this._deployment == null ? null : this._deployment.PackageContent.Bootstrapper.Assembly.Version; }
		}

		public string StatePath {
			get { return (Path.Combine(this._deployment.SettingsFolder, "appstate.bin")); }
		}

		/// <summary>
		/// Alternative to deploying a package is to start an application from a know deployed location.
		/// </summary>
		public void LoadFrom(string directory) {
			Logger.Info(this, "Loading from directory " + directory + "...");
			string deploymentInfoPath = Path.Combine(directory, "deployment.info");
			if(File.Exists(deploymentInfoPath)) {
				using(FileStream stream = File.Open(deploymentInfoPath, FileMode.Open, FileAccess.Read)) {
					Deployment deployment = XmlSerializer.Deserialize<Deployment>(stream);
					this._deployment = deployment;
					Logger.Info(this, "Loaded deployment info from " + deploymentInfoPath);
				}
			} else {
				this._deployment = new Deployment { PackageContent = this._packageRepository.ExtractPackageInfo(directory) };
				this._deployment.BinFolder = Path.Combine(this._context.AppFolder, this.Name, "bin", this._deployment.PackageContent.Bootstrapper.Assembly.Version);
				this._deployment.SettingsFolder = Path.Combine(this._context.AppFolder, this.Name, "conf");
				this._deployment.SettingsPath = Path.Combine(this._deployment.SettingsFolder, this._deployment.PackageContent.Manifest.ConfigurationFile);
			}
			this.Load();
		}

		/// <summary>
		/// The physical moving of files in a <see cref="Package"/> to a location from where they can be executed.
		/// </summary>
		/// <param name="package"></param>
		/// <returns></returns>
		public Task Deploy(Package package) {
			return (this._server.Run(() => {
				// TODO [SAR] potential dead lock with the server action queue, is this lock required?
				lock(this._lock) {
					Logger.Info(this, "Deploying package from " + package.Source + "...");
					this._deployment = new Deployment { PackageContent = this._packageRepository.ExtractPackageInfo(package) };
					this._deployment.BinFolder = Path.Combine(this._context.AppFolder, this.Name, "bin", this._deployment.PackageContent.Bootstrapper.Assembly.Version);
					this._deployment.SettingsFolder = Path.Combine(this._context.AppFolder, this.Name, "conf");
					package.Deploy(this._deployment);
					string deploymentInfoPath = Path.Combine(this._deployment.BinFolder, "deployment.info");
					using(FileStream stream = File.Create(deploymentInfoPath)) {
						XmlSerializer.Serialize(this._deployment, stream, new NamespaceMapping { Prefix = "", Namespace = Serialization.NAMESPACE });
                        Logger.Info(this, "Saved deployment info to " + deploymentInfoPath);
					}
					this.RunUpdates();
					this.Load();
					Logger.Info(this, "Package deployed");
				}
			}));
		}

		/// <summary>
		/// Maximum command line length in Windows (> XP) = 8191 characters.
		/// </summary>
		public Task Start(params string[] args) {
			this.SaveArgs(args);
			return (this._server.Run(() => {
				lock (this._lock) {
					Logger.Info(this, "Starting...");
					if (this._applicationHost == null) throw new Exception("Bootstrapper not set");
					this._applicationHost.Initialize(this._context, this._deployment.PackageContent.Bootstrapper, args);
					this._applicationHost.Start();
					Logger.Info(this, "Started");
				}
			}));
		}

		public Task Stop() {
			return (this._server.Run(() => {
				lock (this._lock) {
					Logger.Info(this, "Stopping...");
					try {
						if (this._applicationHost == null) {
							Logger.Warn(this, "Application " + this._name + " cannot be safely stopped because application host is not set");
							return;
						}
						this._applicationHost.Stop();
						Logger.Info(this, "Stopped");
					} catch (Exception ex) {
						Logger.Error(this, ex, "Stopping application " + this._name + " failed");
					}
				}
			}));
		}

		/// <summary>
		/// Same as <see cref="Start"/>, but first loads the arguments that were initially supplied to <see cref="Start"/>, useful
		/// for restarting and so on.
		/// </summary>
		/// <returns></returns>
		internal Task ReStart() {
			string[] args = this.LoadArgs();
			return (this.Start(args));
		}

		internal Task Unload() {
			return (this._server.Run(() => {
				if(this._applicationHost == null) {
					Logger.Warn(this, "Unloaded already");
					return;
				}
				this._applicationHost.Destroy();
				Logger.Info(this, "Unloaded");
			}));
		}

		private void RunUpdates() {
			AppDomain appDomain = null;
			try {
				appDomain = ReflectionHelper.LoadAppDomain(this._deployment.BinFolder, this._deployment.PackageContent.Bootstrapper.Assembly.File);
				foreach(Contract.Type updaterInfo in this._deployment.PackageContent.Updaters) {
					try {
						Updater updater = ReflectionHelper.CreateInstance<Updater>(appDomain, updaterInfo);
						updater.Run(this._context);
					} catch(Exception ex) {
						Logger.Error("Failed to run updater " + updaterInfo.FullName + " because: " + ex.GetRootCause().Message);
					}
				}
			} catch(Exception ex) {
				Logger.Warn(this, "Failed to run updaters: " + ex.Message);
				throw;
			} finally {
				if(appDomain != null) AppDomain.Unload(appDomain);
			}
		}

		private void Load() {
			Logger.Info(this, "Loading...");
			this._applicationHost = this._hostingModel.Create(this._deployment.BinFolder, this._deployment.PackageContent.Bootstrapper.Assembly.File);
			Logger.Info(this, "Loaded");
		}

		private string[] LoadArgs() {
			try {
				Logger.Info(this, "Loading application args from " + this.StatePath + "...");
				if(!File.Exists(this.StatePath)) {
					Logger.Warn(this, "Application args file could not be found");
					return (null);
				}
				List<string> lines = new List<string>();
				using (StreamReader file = new StreamReader(this.StatePath)) {
					string line;
					while((line = file.ReadLine()) != null) {
						string decoded = this.Base64Decode(line);
						lines.Add(decoded);
					}
					file.Close();
				}
				return (lines.ToArray());
			} catch(Exception ex) {
				Logger.Error(this, ex, "Could not save application args");
			}
			return (null);
		}

		private void SaveArgs(string[] args) {
			try {
				Logger.Info(this, "Saving application args to " + this.StatePath + "...");
				if(File.Exists(this.StatePath)) File.Delete(this.StatePath);
				string directory = Path.GetDirectoryName(this.StatePath);
				if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory)) Directory.CreateDirectory(directory);
				using(StreamWriter file = new StreamWriter(this.StatePath)) {
					if (args != null) {
						foreach (string arg in args) {
							string encoded = this.Base64Encode(arg);
							file.WriteLine(encoded);
						}
					}
					file.Close();
				}
			} catch(Exception ex) {
				Logger.Error(this, ex, "Could not save application args");
			}
		}

		private string Base64Encode(string plainText) {
			var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
			return Convert.ToBase64String(plainTextBytes);
		}

		private string Base64Decode(string base64EncodedData) {
			var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
			return Encoding.UTF8.GetString(base64EncodedBytes);
		}

	}

}
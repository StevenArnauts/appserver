using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Utilities;

namespace Core.Persistence {

	public class FileSystemRepository : IApplicationRepository {

		private readonly string _rootFolder;
		private readonly string _tempFolder;

		/// <summary>
		/// Knows how to read and write applications and packages to and from disk.
		/// </summary>
		public FileSystemRepository(IApplicationRepositoryConfiguration config) {
			this._rootFolder = Path.IsPathRooted(config.RootFolder) ? config.RootFolder : Path.GetFullPath(config.RootFolder);
			if (!Directory.Exists(this._rootFolder)) Directory.CreateDirectory(this._rootFolder);
			this._tempFolder = Path.IsPathRooted(config.TempFolder) ? config.TempFolder : Path.GetFullPath(config.TempFolder);
			if(!Directory.Exists(this._tempFolder)) Directory.CreateDirectory(this._tempFolder);
		}

		public string RootFolder {
			get { return this._rootFolder; }
		}

		public string TempFolder {
			get { return this._tempFolder; }
		}

		public IEnumerable<FileSystemApplication> GetApplications() {
			return (Directory.GetDirectories(this._rootFolder).Select(d => new FileSystemApplication { Name = Path.GetFileName(d), Directory = Path.GetFullPath(d) }));
		}

		public FileSystemApplication GetApplication(string name) {
			string directory = Path.Combine(this._rootFolder, name);
			if (!Directory.Exists(directory)) throw new ObjectNotFoundException("Application " + name + " does not exist");
			return (new FileSystemApplication { Name = name, Directory = directory });
		}

		public IEnumerable<FileSystemPackage> GetPackages(FileSystemApplication application, Version since = null) {
			List<FileSystemPackage> packages = new List<FileSystemPackage>();
			foreach(string packageFolder in Directory.EnumerateDirectories(Path.Combine(application.Directory, "bin"))) {
				Version version;
				string packageFolderName = Path.GetFileName(packageFolder);
				if (!Version.TryParse(packageFolderName, out version)) {
					Logger.Warn(this, "Package folder " + packageFolder + " could not be parsed as a " + typeof(Version).FullName);
					continue;
				}
				if (since == null || version > since) {
					FileSystemPackage package = new FileSystemPackage {
						Version = version,
						Directory = Path.GetFullPath(packageFolder),
						Application = application
					};
					packages.Add(package);
				}
			}
			return (packages.OrderBy(p => p.Version));
		}

		public FileSystemPackage GetPackage(FileSystemApplication application, Version version = null) {
			List<FileSystemPackage> packages = this.GetPackages(application).OrderBy(p => p.Version).ToList();
			if (packages.Count > 0) {
				if(version != null) {
					FileSystemPackage package = packages.FirstOrDefault(p => p.Version == version);
					if(package != null) return (package);
				} else {
					FileSystemPackage package = packages.Last();
					return (package);
				}
			}
			throw new ObjectNotFoundException("Package version " + version + " does not exist for application " + application.Name);
		}

		public PackageContent ExtractPackageInfo(Package package) {
			PackageContent packageContent = null;
            string tempPath = Path.Combine(this._tempFolder, Guid.NewGuid().ToString("N").ToUpper());
			try {
				package.Extract(tempPath);
				packageContent = this.ExtractPackageInfo(tempPath);
			} finally {
				try {
					Directory.Delete(tempPath, true);
				} catch(Exception ex) {
					Logger.Warn(this, "Failed to remove temp folder " + tempPath + ": " + ex.Message);
				}
			}
			return (packageContent);
		}

		public PackageContent ExtractPackageInfo(string directory) {
			AppDomain tempDomain = Utilities.AppDomainManager.LoadAppDomain(directory, new Uri(this.GetType().Assembly.CodeBase).LocalPath);
			try {
				object handle;
				try {
					handle = tempDomain.CreateInstanceAndUnwrap(typeof(PackageScanner).Assembly.GetName().Name, typeof(PackageScanner).FullName);
				} catch(FileNotFoundException) {
					throw new AppServerException("Package does not contain required assembly " + typeof(PackageScanner).Assembly.Name());
				}
				PackageScanner scanner = handle as PackageScanner;
				if(scanner == null) throw new Exception("Could not load package scanner");
				PackageContent info = scanner.Run();
				Logger.Info(this, "Scanner found " + info.Bootstrappers.Count + " bootstrapper(s) and " + info.Updaters.Count + " updater(s)");
				string manifestFile = Path.Combine(directory, "manifest.xml");
				if(File.Exists(manifestFile)) {
					Logger.Debug(this, "Loading package manifest " + manifestFile + "...");
					try {
						using(FileStream stream = File.Open(manifestFile, FileMode.Open, FileAccess.Read)) {
							info.Manifest = XmlSerializer.Deserialize<Manifest>(stream);
							Logger.Debug(this, "Manifest loaded");
						}
					} catch(Exception ex) {
						Logger.Error("Failed to load manifest: " + ex.Message);
					}
				} else {
					Logger.Debug(this, "Package contains no manifest");
				}
				return (info);
			} finally {
				AppDomain.Unload(tempDomain);
			}
		}

	}

}
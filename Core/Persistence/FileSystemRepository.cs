using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Utilities;

namespace Core.Persistence {

	public class FileSystemRepositoryConfiguration : IApplicationRepositoryConfiguration {

		private readonly string _rootFolder;
		private readonly string _tempFolder;

		public FileSystemRepositoryConfiguration(string rootFolder, string tempFolder) {
			this._rootFolder = rootFolder;
			this._tempFolder = tempFolder;
		}

		public string RootFolder {
			get { return this._rootFolder; }
		}

		public string TempFolder {
			get { return this._tempFolder; }
		}

	}

	public class FileSystemRepository : IApplicationRepository {

		private readonly string _rootFolder;
		private readonly string _tempFolder;

		/// <summary>
		/// Knows how to read and write applications and packages to and from disk.
		/// </summary>
		public FileSystemRepository(IApplicationRepositoryConfiguration config) {
			this._rootFolder = Path.IsPathRooted(config.RootFolder) ? config.RootFolder : Path.GetFullPath(config.RootFolder);
			this._tempFolder = Path.IsPathRooted(config.TempFolder) ? config.TempFolder : Path.GetFullPath(config.TempFolder);
		}

		public string RootFolder {
			get { return this._rootFolder; }
		}

		public string TempFolder {
			get { return this._tempFolder; }
		}

		public FileSystemApplication CreateApplication(string name) {
			string path = Path.Combine(this._rootFolder, name);
			if(Directory.Exists(path)) throw new InvalidOperationException("Application " + name + " already exists");
			Directory.CreateDirectory(path);
			return (new FileSystemApplication { Name = name, Directory = path });
		}

		public IEnumerable<FileSystemApplication> GetApplications() {
			return (Directory.GetDirectories(this._rootFolder).Select(d => new FileSystemApplication { Name = Path.GetFileName(d), Directory = Path.GetFullPath(d) }));
		}

		public FileSystemApplication GetApplication(string name) {
			string directory = Path.Combine(this._rootFolder, name);
			if (!Directory.Exists(directory)) throw new ObjectNotFoundException("Application " + name + " does not exist");
			return (new FileSystemApplication { Name = name, Directory = directory });
		}

		public FileSystemPackage CreatePackage(FileSystemApplication application, Package source) {
			IEnumerable<FileSystemPackage> existingPackages = this.GetPackages(application);
			PackageContent packageContentInfo;
			try {
				 packageContentInfo = this.ExtractPackageInfo(source);
			} catch (AppServerException ex) {
				Logger.Warn(this, ex, "Unable to extract package info");
				throw new InvalidOperationException(ex.Message);
			}
			FileSystemPackage package = new FileSystemPackage { Version = Version.Parse(packageContentInfo.Bootstrapper.Assembly.Version) };
			Logger.Info(this, "Detected package version = " + package.Version);
			if(existingPackages.Any(p => p.Version == package.Version)) throw new InvalidOperationException("Application " + application.Name + " already has a package with version " + package.Version.ToString(4));
			package.Application = application;
			package.File = Path.Combine(this._rootFolder, application.Name, package.Version.ToString(4), source.Source);
			package.Directory = Path.GetDirectoryName(package.File);
			source.SaveAs(package.File);
			return (package);
		}

		public IEnumerable<FileSystemPackage> GetPackages(FileSystemApplication application, Version since = null) {
			List<FileSystemPackage> packages = new List<FileSystemPackage>();
			foreach (string packageFolder in Directory.EnumerateDirectories(application.Directory)) {
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
					string file = Directory.GetFiles(packageFolder, "*.zip").FirstOrDefault();
					if(!string.IsNullOrEmpty(file)) {
						package.Timestamp = File.GetLastWriteTime(file);
						package.File = file;
						packages.Add(package);
					}
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

		public Stream GetPackage(FileSystemPackage package) {
			if(string.IsNullOrEmpty(package.File)) throw new ObjectNotFoundException("Package file not found for " + package.Application.Name + " " + package.Version);
			return (File.OpenRead(package.File));
		}

		public PackageContent ExtractPackageInfo(Package package) {
			string tempPath = Path.Combine(this._tempFolder, Guid.NewGuid().ToString("N").ToUpper());
			package.Extract(tempPath);
			AppDomain tempDomain = Utilities.AppDomainManager.LoadAppDomain(tempPath, new Uri(this.GetType().Assembly.CodeBase).LocalPath);
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
				string manifestFile = Path.Combine(tempPath, "manifest.xml");
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
				try {
					Directory.Delete(tempPath, true);
				} catch(Exception ex) {
					Logger.Warn(this, "Failed to remove temp folder " + tempPath + ": " + ex.Message);
				}
			}
		}

	}

}
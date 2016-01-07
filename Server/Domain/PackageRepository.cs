using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core;
using Utilities;

namespace Server.Domain {

	public class PackageRepository : IPackageRepository {

		private readonly string _rootFolder;
		private readonly string _tempFolder;

		public PackageRepository(IRepositoryConfiguration repositoryConfiguration) {
			this._rootFolder = Path.IsPathRooted(repositoryConfiguration.RootFolder) ? repositoryConfiguration.RootFolder : Path.GetFullPath(repositoryConfiguration.RootFolder);
			this._tempFolder = Path.IsPathRooted(repositoryConfiguration.TempFolder) ? repositoryConfiguration.TempFolder : Path.GetFullPath(repositoryConfiguration.TempFolder);
		}

		public Application CreateApplication(string name) {
			string path = Path.Combine(this._rootFolder, name);
			if(Directory.Exists(path)) throw new InvalidOperationException("Application " + name + " already exists");
			Directory.CreateDirectory(path);
			return (new Application { Name = name, Directory = path });
		}

		public IEnumerable<Application> GetApplications() {
			return (Directory.GetDirectories(this._rootFolder).Select(d => new Application { Name = Path.GetFileName(d), Directory = Path.GetFullPath(d) }));
		}

		public Application GetApplication(string name) {
			string directory = Path.Combine(this._rootFolder, name);
			if (!Directory.Exists(directory)) throw new ObjectNotFoundException("Application " + name + " does not exist");
			return (new Application { Name = name, Directory = directory });
		}

		public Package CreatePackage(Application application, Core.Package source) {
			IEnumerable<Package> existingPackages = this.GetPackages(application);
			Core.Contract.Package packageInfo;
			try {
				 packageInfo = source.ExtractPackageInfo(this._tempFolder);
			} catch (AppServerException ex) {
				Logger.Warn(this, ex, "Unable to extract package info");
				throw new InvalidOperationException(ex.Message);
			}
			Package package = new Package { Version = Version.Parse(packageInfo.Bootstrapper.Assembly.Version) };
			Logger.Info(this, "Detected package version = " + package.Version);
			if(existingPackages.Any(p => p.Version == package.Version)) throw new InvalidOperationException("Application " + application.Name + " already has a package with version " + package.Version.ToString(4));
			package.Application = application;
			package.File = Path.Combine(this._rootFolder, application.Name, package.Version.ToString(4), source.Source);
			package.Directory = Path.GetDirectoryName(package.File);
			source.SaveAs(package.File);
			return (package);
		}

		public IEnumerable<Package> GetPackages(Application application, Version since = null) {
			List<Package> packages = new List<Package>();
			foreach (string packageFolder in Directory.EnumerateDirectories(application.Directory)) {
				Version version;
				string packageFolderName = Path.GetFileName(packageFolder);
				if (!Version.TryParse(packageFolderName, out version)) {
					Logger.Warn(this, "Package folder " + packageFolder + " could not be parsed as a " + typeof(Version).FullName);
					continue;
				}
				if (since == null || version > since) {
					Package package = new Package {
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

		public Package GetPackage(Application application, Version version = null) {
			List<Package> packages = this.GetPackages(application).OrderBy(p => p.Version).ToList();
			if (packages.Count > 0) {
				if(version != null) {
					Package package = packages.FirstOrDefault(p => p.Version == version);
					if(package != null) return (package);
				} else {
					Package package = packages.Last();
					return (package);
				}
			}
			throw new ObjectNotFoundException("Package version " + version + " does not exist for application " + application.Name);
		}

		public Stream GetPackage(Package package) {
			if(string.IsNullOrEmpty(package.File)) throw new ObjectNotFoundException("Package file not found for " + package.Application.Name + " " + package.Version);
			return (File.OpenRead(package.File));
		}

	}

}
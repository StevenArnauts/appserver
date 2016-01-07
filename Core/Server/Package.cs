using System;
using System.IO;
using System.IO.Compression;
using Utilities;

namespace Core {

	/// <summary>
	/// The application is assumed to be in a .zip that has to be extracted first, this is the Package. 
	/// The archive is extracted every time the host starts (and all files are overwritten).
	/// </summary>
	public abstract class Package {

		public string Source { get; internal set; }

		protected abstract Stream Read();
		public abstract void SaveAs(string path);

		public void Extract(string targetDirectory) {
			Logger.Debug(this, "Extracting to " + targetDirectory + "...");
			ZipArchive archive = new ZipArchive(this.Read());
			try {
				foreach (ZipArchiveEntry entry in archive.Entries) {
					if (entry.Length == 0) continue;
					this.ExtractEntry(entry, Path.Combine(targetDirectory, entry.FullName), true);
				}
			} finally {
				archive.Dispose();
			}
			Logger.Debug(this, "Extracted");
		}

		public void Deploy(DeploymentInfo deployment) {
			Logger.Debug(this, "Deploying to " + deployment.BinFolder + "...");
			ZipArchive archive = new ZipArchive(this.Read());
			try {
				foreach(ZipArchiveEntry entry in archive.Entries) {
					if(entry.Length == 0) continue;
					if(deployment.PackageInfo.Manifest != null && entry.FullName == deployment.PackageInfo.Manifest.ConfigurationFile) {
						string settingsPath = Path.Combine(deployment.SettingsFolder, entry.Name);
						this.ExtractEntry(entry, settingsPath);
						deployment.SettingsPath = settingsPath;
					} else {
						this.ExtractEntry(entry, Path.Combine(deployment.BinFolder, entry.FullName), true);
					}
				}
			} finally {
				archive.Dispose();
			}
			Logger.Debug(this, "Deployed");
		}

		public Contract.Package ExtractPackageInfo(string tempFolder) {
			this.Extract(tempFolder);
			AppDomain tempDomain = Utilities.AppDomainManager.LoadAppDomain(tempFolder, new Uri(this.GetType().Assembly.CodeBase).LocalPath);
			try {
				object handle;
				try {
					handle = tempDomain.CreateInstanceAndUnwrap(typeof (PackageScanner).Assembly.GetName().Name, typeof (PackageScanner).FullName);
				} catch (FileNotFoundException) {
					throw new AppServerException("Package does not contain required assembly " + typeof (PackageScanner).Assembly.Name());
				}
				PackageScanner scanner = handle as PackageScanner;
				if (scanner == null) throw new Exception("Could not load package scanner");
				Contract.Package info = scanner.Run();
				Logger.Info(this, "Scanner found " + info.Bootstrappers.Count + " bootstrapper(s) and " + info.Updaters.Count + " updater(s)");
				string manifestFile = Path.Combine(tempFolder, "manifest.xml");
				if (File.Exists(manifestFile)) {
					Logger.Debug(this, "Loading package manifest " + manifestFile + "...");
					try {
						using (FileStream stream = File.Open(manifestFile, FileMode.Open, FileAccess.Read)) {
							info.Manifest = XmlSerializer.Deserialize<Manifest>(stream);
							Logger.Debug(this, "Manifest loaded");
						}
					} catch (Exception ex) {
						Logger.Error("Failed to load manifest: " + ex.Message);
					}
				} else {
					Logger.Debug(this, "Package contains no manifest");
				}
				return ( info );
			} finally {
				AppDomain.Unload(tempDomain);
				try {
					Directory.Delete(tempFolder, true);
				} catch(Exception ex) {
					Logger.Warn(this, "Failed to remove temp folder " + tempFolder + ": " + ex.Message);
				}
			}
		}

		private void ExtractEntry(ZipArchiveEntry entry, string destinationPath, bool overwrite = false) {
			string destinationFolder = Path.GetDirectoryName(destinationPath);
			if(destinationFolder != null && !Directory.Exists(destinationFolder)) Directory.CreateDirectory(destinationFolder);
			if(overwrite || !File.Exists(destinationPath)) entry.ExtractToFile(destinationPath, true);
		}

	}

}
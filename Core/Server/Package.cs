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

		public void Deploy(Deployment deployment) {
			Logger.Debug(this, "Deploying to " + deployment.BinFolder + "...");
			ZipArchive archive = new ZipArchive(this.Read());
			try {
				foreach(ZipArchiveEntry entry in archive.Entries) {
					if(entry.Length == 0) continue;
					if(deployment.PackageContent.Manifest != null && entry.FullName == deployment.PackageContent.Manifest.ConfigurationFile) {
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

		private void ExtractEntry(ZipArchiveEntry entry, string destinationPath, bool overwrite = false) {
			string destinationFolder = Path.GetDirectoryName(destinationPath);
			if(destinationFolder != null && !Directory.Exists(destinationFolder)) Directory.CreateDirectory(destinationFolder);
			if(overwrite || !File.Exists(destinationPath)) entry.ExtractToFile(destinationPath, true);
		}

	}

}
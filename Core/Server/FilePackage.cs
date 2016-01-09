using System.IO;
using Utilities;

namespace Core {

	/// <summary>
	/// A package that is a .zip file on the file system.
	/// </summary>
	public class FilePackage : ArchivePackage {

		private string _path;

		public static Package Open(string path) {
			FilePackage package = new FilePackage {
				_path = System.IO.Path.GetFullPath(path),
				Source = path
			};
			return (package);
		}

		public string Path {
			get { return (this._path); }
		}

		public override void SaveAs(string path) {
			Logger.Info(this, "Saving as " + path);
			File.Copy(this._path, path, true);
		}

		protected override Stream Read() {
			return (File.OpenRead(this.Path));
		}

	}

}
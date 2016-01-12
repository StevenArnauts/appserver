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

}
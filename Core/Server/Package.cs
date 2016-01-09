namespace Core {

	/// <summary>
	/// The application is assumed to be in a .zip that has to be extracted first, this is the Package. 
	/// The archive is extracted every time the host starts (and all files are overwritten).
	/// </summary>
	public abstract class Package {

		public string Source { get; internal set; }

		public abstract void SaveAs(string path);
		public abstract void Extract(string targetDirectory);
		public abstract void Deploy(Deployment deployment);

	}

}
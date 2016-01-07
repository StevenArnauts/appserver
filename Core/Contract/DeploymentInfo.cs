namespace Core {

	public class DeploymentInfo {

		/// <summary>
		/// Location where the bin files have been deployed
		/// </summary>
		public string BinFolder { get; internal set; }

		/// <summary>
		/// The folder of the deployed settings file.
		/// </summary>
		public string SettingsFolder { get; internal set; }

		/// <summary>
		/// The path of the deployed settings file
		/// </summary>
		public string SettingsPath { get; internal set; }

		public Contract.Package PackageInfo { get; internal set; }

	}

}
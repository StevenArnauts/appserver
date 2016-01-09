namespace Server.Persistence {

	public interface IApplicationRepositoryConfiguration {

		/// <summary>
		/// The root directory containing the applications
		/// </summary>
		string RootFolder { get; }

		/// <summary>
		/// A temp folder where stuff can be put temporarily
		/// </summary>
		string TempFolder { get; }

	}

}
namespace Server {

	public interface IConfigurationProvider {

		IRepositoryConfiguration Repository { get; }
		IServiceConfiguration Service { get; }

	}

}
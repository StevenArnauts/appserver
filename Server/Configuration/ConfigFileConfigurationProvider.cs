namespace Server {

	public class ConfigFileConfigurationProvider : IConfigurationProvider {

		public IServiceConfiguration Service {
			get { return (ServerConfigurationSection.Current.Service); }
		}

		public IRepositoryConfiguration Repository {
			get { return ( ServerConfigurationSection.Current.Repository ); }
		}

		public IAuthenticationConfiguration Authentication {
			get { return ( ServerConfigurationSection.Current.Authentication ); }
		}

	}

}
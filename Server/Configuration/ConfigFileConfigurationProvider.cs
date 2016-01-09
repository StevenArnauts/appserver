using Server.Persistence;

namespace Server {

	public class ConfigFileConfigurationProvider : IConfigurationProvider {

		public IServiceConfiguration Service {
			get { return (ServerConfigurationSection.Current.Service); }
		}

		public IApplicationRepositoryConfiguration Repository {
			get { return ( ServerConfigurationSection.Current.ApplicationRepository ); }
		}

		public IAuthenticationConfiguration Authentication {
			get { return ( ServerConfigurationSection.Current.Authentication ); }
		}

	}

}
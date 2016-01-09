using System.Configuration;

namespace Server {

	public class ServerConfigurationSection : ConfigurationSection {

		private const string SERVICE_SECTION_NAME = "service";
		private const string REPOSITORY_SECTION_NAME = "repository";
		private const string AUTENTICATION_SECTION_NAME = "authentication";

		public static ServerConfigurationSection Current {
			get { return ( (ServerConfigurationSection)ConfigurationManager.GetSection("server") ); }
		}

		[ConfigurationProperty(REPOSITORY_SECTION_NAME)]
		public ApplicationRepositoryConfigurationElement ApplicationRepository {
			get {
				ApplicationRepositoryConfigurationElement element = (ApplicationRepositoryConfigurationElement)base[REPOSITORY_SECTION_NAME];
				return element;
			}
		}

		[ConfigurationProperty(SERVICE_SECTION_NAME)]
		public ServiceConfigurationElement Service {
			get {
				ServiceConfigurationElement element = (ServiceConfigurationElement)base[SERVICE_SECTION_NAME];
				return element;
			}
		}

		[ConfigurationProperty(AUTENTICATION_SECTION_NAME)]
		public AuthenticationConfigurationElement Authentication {
			get {
				AuthenticationConfigurationElement element = (AuthenticationConfigurationElement)base[AUTENTICATION_SECTION_NAME];
				return element;
			}
		}

	}

}
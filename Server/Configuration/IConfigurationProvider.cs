using Core.Persistence;

namespace Server {

	public interface IConfigurationProvider {

		IApplicationRepositoryConfiguration Repository { get; }
		IServiceConfiguration Service { get; }
		IAuthenticationConfiguration Authentication { get; }

	}

}
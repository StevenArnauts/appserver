using System.Diagnostics.CodeAnalysis;
using System.Web.Http;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Server.Persistence;
using Utilities;

namespace Server {

	[ExcludeFromCodeCoverage]
	public class ContainerConfig {

		public static IContainer Container { get; private set; }

		public static IContainer Initialize() {
			Logger.Info(typeof (ContainerConfig), "Initializing container...");

			ContainerBuilder builder = new ContainerBuilder();

			ConfigFileConfigurationProvider configurationProvider = new ConfigFileConfigurationProvider();
			builder.RegisterInstance(configurationProvider.Service).As<IServiceConfiguration>();
			builder.RegisterInstance(configurationProvider.Repository).As<IApplicationRepositoryConfiguration>();
			builder.RegisterInstance(configurationProvider.Authentication).As<IAuthenticationConfiguration>();

			builder.RegisterType<FileSystemApplicationRepository>().As<IApplicationRepository>();

			builder.RegisterType<SystemDateTimeFactory>().As<IDateTimeFactory>();

			// register API controllers
			builder.RegisterApiControllers(typeof (PackagesApiController).Assembly);
			builder.RegisterControllers(typeof (HomeController).Assembly);

			// build container
			Container = Dependency.Build(builder);
			GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(Container);
			DependencyResolver.SetResolver(new AutofacDependencyResolver(Container));

			Logger.Info(typeof (ContainerConfig), "Container initialized");
			return (Container);
		}

	}

}
using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Server.Configuration;
using Utilities;

namespace Server {

	public class Global : HttpApplication {

		protected void Application_Start(object sender, EventArgs e) {
			Logger.Initialize("log4net.config");
			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			GlobalConfiguration.Configure(WebApiConfig.Configure);
			ContainerConfig.Initialize();
			MapperConfig.Load();
		}

		protected void Session_Start(object sender, EventArgs e) {}

		protected void Application_BeginRequest(object sender, EventArgs e) {}

		protected void Application_AuthenticateRequest(object sender, EventArgs e) {}

		protected void Application_Error(object sender, EventArgs e) {}

		protected void Session_End(object sender, EventArgs e) {}

		protected void Application_End(object sender, EventArgs e) {}

	}

}
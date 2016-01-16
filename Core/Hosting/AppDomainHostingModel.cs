using Utilities;

namespace Core {

	public class AppDomainHostingModel : IHostingModel {

		public IApplicationHost Create(string binFolder, string assembly) {
			AppDomainApplicationHost host = new AppDomainApplicationHost(binFolder, assembly);
			Logger.Info(this, "Created new " + host.GetType().FullName);
			return (host);
		}

	}

}
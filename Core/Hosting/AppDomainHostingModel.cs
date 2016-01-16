using Utilities;

namespace Core {

	public class AppDomainHostingModel : HostingModel {

		internal override ApplicationHost Create(string binFolder, string assembly) {
			AppDomainApplicationHost host = new AppDomainApplicationHost(binFolder, assembly);
			Logger.Info(this, "Created new " + host.GetType().FullName);
			return (host);
		}

	}

}
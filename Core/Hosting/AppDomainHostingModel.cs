using Utilities;

namespace Core {

	public class AppDomainHostingModel : HostingModel {

		internal override ApplicationHost Create(string binFolder, string assembly) {
			Logger.Info(this, "Loading...");
			AppDomainApplicationHost host = new AppDomainApplicationHost(binFolder, assembly);
			Logger.Info(this, "Loaded");
			return (host);
		}

	}

}
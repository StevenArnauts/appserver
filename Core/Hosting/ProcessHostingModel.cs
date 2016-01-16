using Utilities;

namespace Core {

	public class ProcessHostingModel : HostingModel {

		internal override ApplicationHost Create(string binFolder, string assembly) {
			ProcessApplicationHost host = new ProcessApplicationHost(binFolder, assembly);
			Logger.Info(this, "Created new " + host.GetType().FullName);
			return (host);
		}

	}

}
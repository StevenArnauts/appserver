using System.Collections.Generic;
using System.IO;

namespace Core {

	public class ServerFactory {

		private const string DEFAULT_APP_FOLDER = @".\apps";
		private static readonly List<string> rootFolders = new List<string>(); 

		public static Server Create(IHostingModel hostingModel, string appFolder = null, string tempFolder = null) {
			string root = Path.GetFullPath(appFolder ?? DEFAULT_APP_FOLDER);
			if(rootFolders.Contains(root)) throw new AppServerException("There already a server with app root " + root);
			rootFolders.Add(root);
			string temp = Path.GetFullPath(tempFolder ?? Path.Combine(Path.GetTempPath(), "Kluwer", "Install"));
			Server server = new Server(hostingModel, root, temp);
			return (server);
		}

	}

}
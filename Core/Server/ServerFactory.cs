using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Utilities;

namespace Core {

	public class ServerFactory {

		private const string DEFAULT_APP_FOLDER = @".\apps";
		private static readonly List<string> rootFolders = new List<string>(); 

		public static Server Create(HostingModel hostingModel, string appFolder = null, string tempFolder = null) {
			string root = Path.GetFullPath(appFolder ?? DEFAULT_APP_FOLDER);
			if(rootFolders.Contains(root)) throw new AppServerException("There already a server with app root " + root);
			rootFolders.Add(root);
			KillOrphanedHostProcesses();
			string temp = Path.GetFullPath(tempFolder ?? Path.Combine(Path.GetTempPath(), "Kluwer", "Install"));
			Server server = new Server(hostingModel, root, temp);
			return (server);
		}

		private static void KillOrphanedHostProcesses() {
			string processName = Path.GetFileNameWithoutExtension(typeof (Host.Server).Assembly.Path());
			Logger.Info("Looking for orphaned host processes called " + processName + "...");
			Process[] orphans = Process.GetProcessesByName(processName);
			Logger.Info("Found " + orphans.Length + " orphans(s)");
			foreach (Process orphan in orphans) {
				orphan.Kill();
			}
		}

	}

}
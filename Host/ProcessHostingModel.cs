using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using Utilities;

namespace Core.ProcessHost {

	public class ProcessHostingModel : IHostingModel {
		
		private static readonly PortManager portManager = new PortManager(8000, 9000);

		static ProcessHostingModel() {
			KillOrphanedHostProcesses();
			SetupRemoting(portManager.ClaimNext());
		}

		public IApplicationHost Create(string binFolder, string assembly) {
			ProcessApplicationHost host = new ProcessApplicationHost(binFolder, portManager);
			Logger.Info("Created new " + host.GetType().FullName);
			return (host);
		}

		private static void KillOrphanedHostProcesses() {
			string processName = Path.GetFileNameWithoutExtension(typeof(Server).Assembly.Path());
			Logger.Info("Looking for orphaned host processes called " + processName + "...");
			Process[] orphans = Process.GetProcessesByName(processName);
			Logger.Info("Found " + orphans.Length + " orphans(s)");
			foreach(Process orphan in orphans) {
				orphan.Kill();
			}
		}

		private static void SetupRemoting(int port) {
			BinaryClientFormatterSinkProvider clientProvider = new BinaryClientFormatterSinkProvider();
			BinaryServerFormatterSinkProvider serverProvider = new BinaryServerFormatterSinkProvider { TypeFilterLevel = TypeFilterLevel.Full };
			IDictionary props = new Hashtable();
			props["port"] = port;
			Logger.Info("Initializing on port " + port + "...");
			TcpChannel chan = new TcpChannel(props, clientProvider, serverProvider);
			ChannelServices.RegisterChannel(chan, false);
		}

	}

}
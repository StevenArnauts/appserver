using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using Utilities;

namespace Core.ProcessHost {

	public class ProcessHostingModel : IHostingModel {

		static ProcessHostingModel() {
			KillOrphanedHostProcesses();
			SetupRemoting();
		}

		public IApplicationHost Create(string binFolder, string assembly) {
			ProcessApplicationHost host = new ProcessApplicationHost(binFolder);
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

		private static void SetupRemoting() {
			// BinaryClientFormatterSinkProvider clientProvider = new BinaryClientFormatterSinkProvider();
			// BinaryServerFormatterSinkProvider serverProvider = new BinaryServerFormatterSinkProvider { TypeFilterLevel = TypeFilterLevel.Full };

			SoapClientFormatterSinkProvider clientProvider = new SoapClientFormatterSinkProvider();
			SoapServerFormatterSinkProvider serverProvider = new SoapServerFormatterSinkProvider();

			IDictionary props = new Hashtable();
			int port = IPHelper.FindFreePort(8000, 8500);
			props["port"] = port;
			Logger.Info("Initializing on port " + port + "...");
			TcpChannel chan = new TcpChannel(props, clientProvider, serverProvider);
			ChannelServices.RegisterChannel(chan, false);
		}

	}

}
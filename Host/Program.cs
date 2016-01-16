using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace Host {

	public class Program {

		private static void Main(string[] args) {
			try {
				Logger.Log("Starting...");
				ServerConfiguration configuration = ParseArguments(args);
				StartServer(configuration);
				Logger.Log("Started");
			} catch (Exception ex) {
				Logger.Log("Startup failed", ex);
			}
			Logger.Log("Press <ENTER> to exit...");
			Console.ReadLine();
		}

		private static ServerConfiguration ParseArguments(string[] args) {
			ServerConfiguration config = new ServerConfiguration();
			if (args.Length < 1) throw new Exception("Expected 1 argument");
			int port;
			if (!int.TryParse(args[0], out port)) throw new Exception("Argument 1 could not be parsed as an int");
			config.Port = port;
			return (config);
		}

		private static void StartServer(ServerConfiguration config) {
			TcpChannel channel = new TcpChannel(config.Port);
			ChannelServices.RegisterChannel(channel, false);
			RemotingConfiguration.RegisterWellKnownServiceType(typeof(Server), Server.OBJECT_URI, WellKnownObjectMode.Singleton);
			foreach (var o in RemotingConfiguration.GetRegisteredWellKnownServiceTypes()) {
				Logger.Log(o.TypeName + " is ready for requests on tcp://localhost:" + config.Port + o.ObjectUri);
			}
			
		}

	}

}
using System;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using Utilities;

namespace Core.ProcessHost {

	public class Program {

		private static void Main(string[] args) {
			try {
				Logger.Initialize("log4net.config");
				Logger.Info("Starting in directory " + AppDomain.CurrentDomain.BaseDirectory + "...");
				ServerConfiguration configuration = ParseArguments(args);
				StartServer(configuration);
				Logger.Info("Started");
			} catch (Exception ex) {
				Logger.Error("Startup failed", ex);
			}
			Logger.Info("Press <ENTER> to exit...");
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
			//BinaryClientFormatterSinkProvider clientProvider = new BinaryClientFormatterSinkProvider();
			//BinaryServerFormatterSinkProvider serverProvider = new BinaryServerFormatterSinkProvider { TypeFilterLevel = TypeFilterLevel.Full };
			SoapClientFormatterSinkProvider clientProvider = new SoapClientFormatterSinkProvider();
			SoapServerFormatterSinkProvider serverProvider = new SoapServerFormatterSinkProvider();
			IDictionary props = new Hashtable();
			props["port"] = config.Port;
			TcpChannel chan = new TcpChannel(props, clientProvider, serverProvider);
			ChannelServices.RegisterChannel(chan, false);
			RemotingConfiguration.RegisterWellKnownServiceType(typeof(Server), Server.OBJECT_URI, WellKnownObjectMode.Singleton);
			foreach (var o in RemotingConfiguration.GetRegisteredWellKnownServiceTypes()) {
				Logger.Info(o.TypeName + " is listening on tcp://localhost:" + config.Port + "/" + o.ObjectUri);
			}
			
		}

	}

}
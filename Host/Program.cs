using System;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using Utilities;

namespace Host {

	public class Program {

		private static void Main(string[] args) {
			try {
				Logger.Initialize("log4net.config");
				Logger.Info("Starting...");
				ServerConfiguration configuration = ParseArguments(args);
				StartServer(configuration);
				Logger.Info("Started");
			} catch (Exception ex) {
				Logger.Error(ex, "Could not start server");
			}
			Console.WriteLine("Press <ENTER> to exit...");
			Console.ReadLine();
		}

		private static ServerConfiguration ParseArguments(string[] args) {
			ServerConfiguration config = new ServerConfiguration();
			if(args.Length < 1) throw new Exception("Expected 1 argument");
			int port;
			if(!int.TryParse(args[0], out port)) throw new Exception("Argument 1 could not be parsed as an int");
			config.Port = port;
			return (config);
		}

		private static void StartServer(ServerConfiguration config) {
			RemotingService service = new RemotingService(config.Port);
			service.Start();
		}

	}

	public class ServerConfiguration {

		public int Port { get; set; }

	}

	public class RemotingService {

		private readonly int _port;

		public RemotingService(int port) {
			this._port = port;
			// Dependency.Register("RemotingService", this);
		}

		public void Start() {
			IDictionary properties = new Hashtable { { "port", this._port }, { "secure", true }, { "impersonate", false } };
			TcpChannel channel = new TcpChannel(properties, null, null);
			ChannelServices.RegisterChannel(channel, true);
			RemotingConfiguration.RegisterWellKnownServiceType(typeof (RemotingEndpoint), "messageservice.rem", WellKnownObjectMode.Singleton);
			Logger.Info(this, "Listening on port " + this._port);
		}

		//public string HandleMessage(InternalEnvelope payload) {
		//	Logger.Info(this, "Reveived message from " + payload.Sender);
		//	if(string.IsNullOrEmpty(payload.Id)) payload.Id = Guid.NewGuid().ToString("N");
		//	Message msg = new Message();
		//	msg.Xml = payload.Serialize();
		//	using(TransactionScope scope = new TransactionScope()) {
		//		this.Publish(msg);
		//		scope.Complete();
		//		return (payload.Id);
		//	}
		//}

		protected void Stop() {
			// stop the TCP server (how?)
		}

		/// <summary>
		/// The endpoint clients can use to send message TO Sigma
		/// </summary>
		private class RemotingEndpoint : MarshalByRefObject {

			public string Process(object message) {
				return (Dependency.Resolve<RemotingService>().ToString());
			}

		}

	}

}
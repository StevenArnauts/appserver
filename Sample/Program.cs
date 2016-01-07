using System;
using Core;
using Utilities;

namespace Sample {

	internal class Program {

		private static void Main() {
			try {

				Logger.Initialize("log4net.config");
				Logger.Info(typeof(Program), "Started");

				Server server = Server.Create(@".\apps", @".\temp");
				
				Application briljant = server.CreateApplication("Briljant");
				briljant.Deploy(FilePackage.Open(@"..\..\Briljant\briljant.zip")).Wait();
				briljant.Start();

				Application alure = server.CreateApplication("Alure");
				alure.Deploy(FilePackage.Open(@"..\..\Alure\alure.zip"));
				alure.Start();

				Application cloudbox = server.CreateApplication("Cloudbox");
				cloudbox.Deploy(FilePackage.Open(@"..\..\Cloudbox\cloudbox.zip"));
				cloudbox.Start();

				PackageWatcher alureWatcher = new PackageWatcher(@"..\..\Alure", "alure.zip", alure);
				alureWatcher.Start();

				ServerWatcher briljantPoller = new ServerWatcher(new Uri("https://localhost:44300"), briljant);
				briljantPoller.Start();

				Console.WriteLine("Press <ENTER> to stop...");
				Console.ReadLine();

				alureWatcher.Stop();
				briljantPoller.Stop();
				server.Stop();
				server.Dispose();

			} catch (Exception ex) {
				Logger.Error(ex);
			}

			Console.WriteLine("Press <ENTER> to exit...");
			Console.ReadLine();
		}

	}

}
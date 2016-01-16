using System;
using System.Collections.Generic;
using Core;
using Utilities;

namespace Sample {

	internal class Program {

		private static void Main() {
			try {

				Logger.Initialize("log4net.config");
				Logger.Info(typeof(Program), "Started");

				Server server = ServerFactory.Create(new ProcessHostingModel(), @".\apps", @".\temp");
				IEnumerable<Application> existingApplications = server.Load();
				foreach (Application existing in existingApplications) {
					existing.Start();
				}

				Application briljant;
				if (!server.TryGetApplication("Briljant", out briljant)) {
                    briljant = server.CreateApplication("Briljant");
					briljant.Deploy(FilePackage.Open(@"..\..\Briljant\briljant.zip")).Wait();
					briljant.Start();
				}

				//Application alure;
				//if (!server.TryGetApplication("Alure", out alure)) {
				//	alure = server.CreateApplication("Alure");
				//	alure.Deploy(FilePackage.Open(@"..\..\Alure\alure.zip"));
				//	alure.Start();
				//}

				//Application cloudbox;
				//if (!server.TryGetApplication("Cloudbox", out cloudbox)) {
				//	cloudbox = server.CreateApplication("Cloudbox");
				//	cloudbox.Deploy(FilePackage.Open(@"..\..\Cloudbox\cloudbox.zip"));
				//	cloudbox.Start();
				//}

				//PackageWatcher alureWatcher = new PackageWatcher(@"..\..\Alure", "alure.zip", alure);
				//alureWatcher.Start();

				//ServerWatcher briljantPoller = new ServerWatcher(new Uri("https://localhost:44300"), briljant, TimeSpan.FromSeconds(10));
				//briljantPoller.Start();

				Console.WriteLine("Press <ENTER> to stop...");
				Console.ReadLine();

				//alureWatcher.Stop();
				//briljantPoller.Stop();
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
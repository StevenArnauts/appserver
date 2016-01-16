using System;
using System.IO;
using Core.Contract;
using Utilities;

namespace Core {

	public class PackageScanner : MarshalByRefObject {

		public PackageScanner() {
			Logger.Initialize("log4net.config");
			Logger.Info(this,"Initialized in app domain " + AppDomain.CurrentDomain.FriendlyName + ", my version is " + this.GetType().Assembly.GetName().Version);
		}

		public PackageContent Run() {
			PackageContent result = new PackageContent();
			foreach(string file in Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")) {
				try {
					System.Reflection.Assembly assembly = System.Reflection.Assembly.LoadFrom(file);
					foreach (System.Type type in assembly.GetExportedTypes()) {
						if (type != typeof (IBootstrapper) && typeof (IBootstrapper).IsAssignableFrom(type)) result.Bootstrappers.Add(Contract.Type.FromType(type));
						if (type != typeof (Updater) && typeof (Updater).IsAssignableFrom(type)) result.Updaters.Add(Contract.Type.FromType(type));
					}
				} catch (Exception ex) {
					Logger.Warn(this, "Could not scan " + file + " because: " + ex.GetRootCause().Message);
				}
			}
			Console.WriteLine("there are " + result.Bootstrappers.Count + " bootstrappers and " + result.Updaters.Count + " updaters");
			return (result);
		}

	}

}
using System;
using System.IO;
using Core.Contract;
using Utilities;
using Assembly = System.Reflection.Assembly;
using Type = System.Type;

namespace Core {

	public class PackageScanner : MarshalByRefObject {

		public PackageScanner() {
			Logger.Info(this,"Initialized in app domain " + AppDomain.CurrentDomain.FriendlyName + ", my version is " + this.GetType().Assembly.GetName().Version);
		}

		public Contract.Package Run() {
			Contract.Package result = new Contract.Package();
			foreach(string file in Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")) {
				try {
					Assembly assembly = Assembly.LoadFrom(file);
					foreach (Type type in assembly.GetExportedTypes()) {
						if (type != typeof (IBootstrapper) && typeof (IBootstrapper).IsAssignableFrom(type)) result.Bootstrappers.Add(Contract.Type.FromType(type));
						if (type != typeof (Updater) && typeof (Updater).IsAssignableFrom(type)) result.Updaters.Add(Contract.Type.FromType(type));
					}
				} catch (Exception ex) {
					Logger.Warn(this, "Could not scan " + file + " because: " + ex.GetRootCause().Message);
				}
			}
			return (result);
		}

	}

}
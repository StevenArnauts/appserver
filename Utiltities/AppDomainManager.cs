using System;
using System.IO;

namespace Utilities {

	public static class AppDomainManager {

		public static AppDomain LoadAppDomain(string folder, string assemblyFile, string configFile = null) {
			string assemblyPath = Path.Combine(folder, assemblyFile);
			string configurationFile = configFile ?? assemblyPath + ".config";
			AppDomainSetup setupInfo = new AppDomainSetup { ApplicationBase = folder, ConfigurationFile = configurationFile };
			Logger.Debug(typeof(AppDomainManager), "Loading assembly " + assemblyPath);
			AppDomain appDomain = AppDomain.CreateDomain(Path.GetFileNameWithoutExtension(assemblyPath), null, setupInfo);
			return appDomain;
		}

	}

}
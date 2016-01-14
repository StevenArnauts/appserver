using System;
using System.IO;
using System.Reflection;
using Utilities;

namespace Core.Infrastructure {

	public class ReflectionHelper {

		public static AppDomain LoadAppDomain(string folder, string assemblyFile, string configFile = null) {
			string assemblyPath = Path.Combine(folder, assemblyFile);
			string configurationFile = configFile ?? assemblyPath + ".config";
			AppDomainSetup setupInfo = new AppDomainSetup { ApplicationBase = folder, ConfigurationFile = configurationFile };
			Logger.Debug("Loading assembly " + assemblyPath);
			AppDomain appDomain = AppDomain.CreateDomain(Path.GetFileNameWithoutExtension(assemblyPath), null, setupInfo);
			return appDomain;
		}

		public static TInstance CreateSponsoredInstance<TInstance>(AppDomain appDomain, Type type, params object[] parameters) where TInstance : MarshalByRefObject {
			TInstance instance = CreateInstance<TInstance>(appDomain, type, parameters);
			RemoteObjectFactory.Sponsor(instance);
			return (instance);
		}

		public static TInstance CreateInstance<TInstance>(AppDomain appDomain, Type type, params object[] parameters) where TInstance : class {
			string assemblyFile = type.Assembly.File;
			if (assemblyFile == null) throw new ArgumentException("Type must include assembly location");
			string assemblyName = type.Assembly.Name;
			object proxy = appDomain.CreateInstanceAndUnwrap(assemblyName, type.FullName, false, BindingFlags.CreateInstance, null, parameters, null, null);
			TInstance instance = proxy as TInstance;
			if(instance == null) throw new Exception("Type " + type.FullName + " cannot be cast to " + typeof(TInstance).FullName);
			Logger.Info("Created instance of " + type.FullName + " from " + assemblyFile + " in app domain " + appDomain.FriendlyName);
			return (instance);
		}

	}

}
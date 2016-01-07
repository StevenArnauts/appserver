using System;
using System.Reflection;

namespace Core.Infrastructure {

	public class ReflectionHelper {

		public static object Instantiate(AppDomain appDomain, Contract.Type type, object[] parameters) {
			string assemblyFile = type.Assembly.File;
			if (assemblyFile == null) throw new ArgumentException("Type must include assembly location");
			string assemblyName = type.Assembly.Name;
			object proxy = appDomain.CreateInstanceAndUnwrap(assemblyName, type.FullName, false, BindingFlags.CreateInstance, null, parameters, null, null);
			return proxy;
		}

	}

}
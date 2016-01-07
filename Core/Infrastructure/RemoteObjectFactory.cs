using System;
using Core.Infrastructure;

namespace Core {

	public static class RemoteObjectFactory {

		private static readonly SponsorshipManager sponsorshipManager = new SponsorshipManager();

		public static SponsorshipManager SponsorshipManager {
			get { return (sponsorshipManager); }
		}

		public static object Create(AppDomain appDomain, Type type, object[] parameters) {
			var proxy = ReflectionHelper.Instantiate(appDomain, type, parameters);
			MarshalByRefObject handle = proxy as MarshalByRefObject;
			if(handle == null) throw new Exception("Type " + type.FullName + " must be a " + typeof(MarshalByRefObject).FullName);
			sponsorshipManager.Register(handle);
			return ( proxy );
		}

	}

}
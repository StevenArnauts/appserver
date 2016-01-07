using System;
using System.Collections.Generic;
using System.IO;

namespace Server.Domain {

	public interface IPackageRepository {

		Application CreateApplication(string name);
		IEnumerable<Application> GetApplications();
		Application GetApplication(string name);
		Package CreatePackage(Application application, Core.Package source);
		IEnumerable<Package> GetPackages(Application application, Version since = null);
		Package GetPackage(Application application, Version version = null);
		Stream GetPackage(Package package);

	}

}
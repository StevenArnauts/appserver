using System;
using System.Collections.Generic;

namespace Core.Persistence {

	public interface IApplicationRepository {

		IEnumerable<FileSystemApplication> GetApplications();
		FileSystemApplication GetApplication(string name);
		IEnumerable<FileSystemPackage> GetPackages(FileSystemApplication application, Version since = null);
		FileSystemPackage GetPackage(FileSystemApplication application, Version version = null);
		PackageContent ExtractPackageInfo(Package package);
		PackageContent ExtractPackageInfo(string directory);
        string RootFolder { get; }
		string TempFolder { get; }

	}

}
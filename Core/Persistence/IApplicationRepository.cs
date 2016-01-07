using System;
using System.Collections.Generic;
using System.IO;

namespace Core.Persistence {

	public interface IApplicationRepository {

		FileSystemApplication CreateApplication(string name);
		IEnumerable<FileSystemApplication> GetApplications();
		FileSystemApplication GetApplication(string name);
		FileSystemPackage CreatePackage(FileSystemApplication application, Package source);
		IEnumerable<FileSystemPackage> GetPackages(FileSystemApplication application, Version since = null);
		FileSystemPackage GetPackage(FileSystemApplication application, Version version = null);
		Stream GetPackage(FileSystemPackage package);
		PackageContent ExtractPackageInfo(Package package);
		string RootFolder { get; }
		string TempFolder { get; }

	}

}
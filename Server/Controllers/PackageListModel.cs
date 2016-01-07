using System.Collections.Generic;
using Core.Persistence;

namespace Server {

	public class PackageListModel {

		public string Application { get; set; }
		public IEnumerable<FileSystemPackage> Packages { get; set; }

	}

}
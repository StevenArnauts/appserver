using System.Collections.Generic;

namespace Server {

	public class PackageListModel {

		public string Application { get; set; }
		public IEnumerable<Domain.Package> Packages { get; set; }

	}

}
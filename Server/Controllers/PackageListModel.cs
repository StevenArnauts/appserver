using System.Collections.Generic;
using Server.Persistence;

namespace Server {

	public class PackageListModel {

		public string Application { get; set; }
		public IEnumerable<Persistence.Package> Packages { get; set; }

	}

}
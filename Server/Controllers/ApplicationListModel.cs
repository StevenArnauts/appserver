using System.Collections.Generic;
using Server.Persistence;

namespace Server {

	public class ApplicationListModel {

		public IEnumerable<Persistence.Application> Applications { get; set; }

	}

}
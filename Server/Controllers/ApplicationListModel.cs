using System.Collections.Generic;
using Core.Persistence;

namespace Server {

	public class ApplicationListModel {

		public IEnumerable<FileSystemApplication> Applications { get; set; }

	}

}
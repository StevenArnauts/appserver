using System;

namespace Server.Domain {

	public class Package {

		public Application Application { get; set; }
		public Version Version { get; set; }
		public DateTime Timestamp { get; set; }
		public string Directory { get; set; }
		public string File { get; set; }

	}

}
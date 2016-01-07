using System;
using Utilities.WebApi;

namespace Server {

	public class Package : Representation {

		public string Version { get; set; }
		public DateTime Timestamp { get; set; }

	}

}
using System;

namespace Core.Contract {

	[Serializable]
	public class Assembly {

		public string Name { get; set; }
		public string File { get; set; }
		public string Version { get; set; }

	}

}
using System;

namespace Core.Persistence {

	public class FileSystemPackage {

		public FileSystemApplication Application { get; set; }
		public Version Version { get; set; }
		public DateTime Timestamp { get; set; }
		public string Directory { get; set; }
		public string File { get; set; }

	}

}
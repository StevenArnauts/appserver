using System.IO;
using Utilities;

namespace Core {

	public class BytePackage : Package {

		private byte[] _bytes;

		public static Package Create(byte[] bytes, string source) {
			BytePackage package = new BytePackage();
			package._bytes = bytes;
			package.Source = source;
			return (package);
		}

		public byte[] Bytes {
			get { return (this._bytes); }
		}

		public override void SaveAs(string path) {
			Logger.Info(this, "Saving as " + path);
			string directory = Path.GetDirectoryName(path);
			if(!string.IsNullOrEmpty(directory) && !Directory.Exists(directory)) Directory.CreateDirectory(directory);
			File.WriteAllBytes(path, this._bytes);
		}

		protected override Stream Read() {
			Stream stream = new MemoryStream(this._bytes);
			stream.Position = 0;
			return stream;
		}

	}

}
using System.IO;

namespace Utilities {

	public static class StreamExtensions {

		public static byte[] ReadAllBytes(this Stream stream) {
			using (MemoryStream memoryStream = new MemoryStream()) {
				stream.CopyTo(memoryStream);
				return (memoryStream.ToArray());
			}
		}

	}

}
using System.IO;
using Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities;

namespace Tests {

	[TestClass]
	public class ManifestTests {

		[TestMethod]
		public void ShouldDeserialize() {
			using (FileStream stream = File.Open("manifest.xml", FileMode.Open, FileAccess.Read)) {
				Manifest sut = XmlSerializer.Deserialize<Manifest>(stream);
				Assert.IsNotNull(sut);
				Assert.IsTrue(!string.IsNullOrEmpty(sut.ConfigurationFile));
			}
		}

		[TestMethod]
		public void ShouldSerialize() {
			Manifest sut = new Manifest { ConfigurationFile = "settings.xml" };
			string xml = XmlSerializer.Serialize(sut, new NamespaceMapping { Prefix = "", Namespace = Manifest.NAMESPACE });
			Assert.IsFalse(string.IsNullOrEmpty(xml));
		}

	}

}
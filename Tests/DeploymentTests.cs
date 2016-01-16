using System.Threading;
using Core;
using Core.Contract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities;

namespace Tests {

	public class TestBootstrapper : IBootstrapper {

		public void Dispose() {
			throw new System.NotImplementedException();
		}

		public void Initialize(ServerContext context) {
			throw new System.NotImplementedException();
		}

		public void Run(CancellationToken token) {
			throw new System.NotImplementedException();
		}

	}

	public class TestUpdater : Updater {

		public override void Run(ServerContext context) {
			throw new System.NotImplementedException();
		}

	}

	[TestClass]
	public class DeploymentTests {

		//[TestMethod]
		//public void ShouldDeserialize() {
		//	using (FileStream stream = File.Open("manifest.xml", FileMode.Open, FileAccess.Read)) {
		//		Manifest sut = XmlSerializer.Deserialize<Manifest>(stream);
		//		Assert.IsNotNull(sut);
		//		Assert.IsTrue(!string.IsNullOrEmpty(sut.ConfigurationFile));
		//	}
		//}

		[TestMethod]
		public void ShouldSerialize() {

			Deployment sut = new Deployment {
				PackageContent = new PackageContent {
					Manifest = new Manifest { ConfigurationFile = "settings.xml" }
				},
				BinFolder = @"C:\Temp\Updates",
				SettingsPath = @"C:\Temp\Updates\conf\settings.xml"
			};
			sut.PackageContent.Bootstrappers.Add(Type.FromType(typeof(TestBootstrapper)));
			sut.PackageContent.Updaters.Add(Type.FromType(typeof(TestUpdater)));
			string xml = XmlSerializer.Serialize(sut, new NamespaceMapping { Prefix = "", Namespace = Serialization.NAMESPACE });
			Assert.IsFalse(string.IsNullOrEmpty(xml));
		}

	}

}
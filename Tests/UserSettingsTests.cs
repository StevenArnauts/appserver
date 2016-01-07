using System.IO;
using Alure;
using Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities;

namespace Tests {

	[TestClass]
	public class UserSettingsTests {

		[TestMethod]
		public void ShouldDeserialize() {
			using (FileStream stream = File.Open("user.config", FileMode.Open, FileAccess.Read)) {
				UserSettings sut = XmlSerializer.Deserialize<UserSettings>(stream);
				Assert.IsNotNull(sut);
				Assert.IsTrue(!string.IsNullOrEmpty(sut.User.Password));
			}
		}

		[TestMethod]
		public void ShouldSerialize() {
			UserSettings sut = new UserSettings { User = new User { Password = "abc", Id = "john"}, Repository = new Repository{Location = @"C:\Temp\Foo"}};
			string xml = XmlSerializer.Serialize(sut, new NamespaceMapping { Prefix = "", Namespace = UserSettings.NAMESPACE });
			Assert.IsFalse(string.IsNullOrEmpty(xml));
		}

	}

}
using System.Xml.Serialization;

namespace Alure {
	
	[XmlRoot(ElementName = "settings", Namespace = NAMESPACE)]
	public class UserSettings {

		public const string NAMESPACE = "http://www.kluwer.be/applications/alure";

		[XmlElement(ElementName = "user")]
		public User User { get; set; }

		[XmlElement(ElementName = "repository")]
		public Repository Repository { get; set; }

	}

	[XmlType(TypeName = "user", Namespace = UserSettings.NAMESPACE)]
	public class User {

		[XmlElement(ElementName = "id")]
		public string Id { get; set; }

		[XmlElement(ElementName = "password")]
		public string Password { get; set; }

	}

	[XmlType(TypeName = "repository", Namespace = UserSettings.NAMESPACE)]
	public class Repository {

		[XmlElement(ElementName = "location")]
		public string Location { get; set; }

	}

}


using System.Xml.Serialization;

namespace Core {

	/// <summary>
	/// Contains all the information that is gathered during a deployment of a package.
	/// </summary>
	[XmlType(TypeName = "deployment", Namespace = Serialization.NAMESPACE)]
	[XmlRoot(ElementName = "deployment", Namespace = Serialization.NAMESPACE)]
	public class Deployment {

		/// <summary>
		/// Location where the bin files have been deployed
		/// </summary>
		[XmlElement(ElementName = "binFolder")]
		public string BinFolder { get; set; }

		/// <summary>
		/// The path of the deployed settings file
		/// TODO [SAR] Remove this
		/// </summary>
		[XmlElement(ElementName = "settingsFolder")]
		public string SettingsFolder { get; set; }

		/// <summary>
		/// The path of the deployed settings file
		/// </summary>
		[XmlElement(ElementName = "settingsPath")]
		public string SettingsPath { get; set; }

		[XmlElement(ElementName = "packageContent")]
		public PackageContent PackageContent { get; set; }

	}

}
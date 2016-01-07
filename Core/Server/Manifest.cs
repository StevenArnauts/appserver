using System;
using System.Xml.Serialization;

namespace Core {

	[XmlRoot(ElementName = "package", Namespace = NAMESPACE)]
	[Serializable]
	public class Manifest {
		public const string NAMESPACE = "http://www.wkb.be/etnia/appserver/package";

		[XmlElement(ElementName = "config")]
		public string ConfigurationFile { get; set; }

	}

}
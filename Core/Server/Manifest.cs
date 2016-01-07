using System;
using System.Xml.Serialization;

namespace Core {

	public static class Serialization {

		public const string NAMESPACE = "http://www.wkb.be/etnia/appserver/package";

	}

	[XmlRoot(ElementName = "manifest", Namespace = Serialization.NAMESPACE)]
	[XmlType(TypeName = "manifest", Namespace = Serialization.NAMESPACE)]
	[Serializable]
	public class Manifest {

		[XmlElement(ElementName = "config")]
		public string ConfigurationFile { get; set; }

	}

}
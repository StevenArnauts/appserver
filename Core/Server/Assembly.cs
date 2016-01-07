using System;
using System.Xml.Serialization;

namespace Core {

	[XmlType(TypeName = "assembly", Namespace = Serialization.NAMESPACE)]
	[Serializable]
	public class Assembly {

		[XmlElement(ElementName = "name")]
		public string Name { get; set; }

		[XmlElement(ElementName = "file")]
		public string File { get; set; }

		[XmlElement(ElementName = "version")]
		public string Version { get; set; }

	}

}
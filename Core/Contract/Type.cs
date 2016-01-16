using System;
using System.Xml.Serialization;
using Utilities;

namespace Core.Contract {

	[XmlType(TypeName = "type", Namespace = Serialization.NAMESPACE)]
	[Serializable]
	public class Type {

		public static Type FromType(System.Type source) {
			Type name = new Type {
				FullName = source.FullName, 
				Name = source.Name, 
				AssemblyQualifiedName = source.AssemblyQualifiedName,
				Assembly = new Assembly {
					File = source.Assembly.Path(),
					Name = source.Assembly.Name(), 
					Version = source.Assembly.Version()
				}
			};
			return (name);
		}

		public System.Type AsType() {
			System.Type type = System.Type.GetType(this.AssemblyQualifiedName, true);
			return (type);
		}

		[XmlElement(ElementName = "assemblyQualifiedName")]
		public string AssemblyQualifiedName { get; set; }

		[XmlElement(ElementName = "name")]
		public string Name { get; set; }

		[XmlElement(ElementName = "fullname")]
		public string FullName { get; set; }

		[XmlElement(ElementName = "assembly")]
		public Assembly Assembly { get; set; }

	}

}
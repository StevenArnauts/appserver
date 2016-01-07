using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Core {

	/// <summary>
	/// The content of the package, as found by the <see cref="PackageScanner"/>.
	/// </summary>
	[XmlType(TypeName = "package", Namespace = Serialization.NAMESPACE)]
	[Serializable]
	public class PackageContent {

		public PackageContent() {
			this.Bootstrappers = new List<Type>();
			this.Updaters = new List<Type>();
		}

		[XmlElement(ElementName = "manifest")]
		public Manifest Manifest { get; set; }

		[XmlArray(ElementName = "bootstappers")]
		public List<Type> Bootstrappers { get; set; }

		[XmlArray(ElementName = "updaters")]
		public List<Type> Updaters { get; set; }

		[XmlIgnore]
		public Type Bootstrapper {
			get {
				if(!this.Bootstrappers.Any()) throw new InvalidOperationException("Package contains no bootstrapper");
				if(this.Bootstrappers.Count > 1) throw new InvalidOperationException("Package contains " + this.Bootstrappers.Count + " bootstrapper(s)");
				return(this.Bootstrappers.First());
			}
		}

	}

}
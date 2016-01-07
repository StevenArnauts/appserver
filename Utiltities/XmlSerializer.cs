using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Utilities {

	public class NamespaceMapping {

		public string Prefix { get; set; }
		public string Namespace { get; set; }

	}

	/// <summary>
	/// Writes xml without xml document declaration
	/// </summary>
	public class XmlFragmentWriter : XmlTextWriter {

		public XmlFragmentWriter(TextWriter w) : base(w) {}
		public override void WriteStartDocument() {}
		public override void WriteStartDocument(bool standalone) {}

	}

	public static class XmlSerializer {

		public static string Serialize<TType>(TType source, params NamespaceMapping[] namespaceMappings) {
			using (MemoryStream ms = new MemoryStream()) {
				using (StreamWriter writer = new StreamWriter(ms, Encoding.UTF8)) {
					XmlFragmentWriter tw = new XmlFragmentWriter(writer) { Formatting = Formatting.Indented, IndentChar = '\t', Indentation = 1 };
					System.Xml.Serialization.XmlSerializer s = new System.Xml.Serialization.XmlSerializer(typeof (TType));
					if (namespaceMappings != null) {
						XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
						foreach (NamespaceMapping nsm in namespaceMappings) {
							ns.Add(nsm.Prefix, nsm.Namespace);
						}
						s.Serialize(tw, source, ns);
					} else {
						s.Serialize(tw, source);
					}
					ms.Position = 0;
					return (new StreamReader(ms).ReadToEnd());
				}
			}
		}

		public static string Serialize(object source, params NamespaceMapping[] namespaceMappings) {
			using (MemoryStream ms = new MemoryStream()) {
				using (StreamWriter writer = new StreamWriter(ms, Encoding.UTF8)) {
					Serialize(source, ms, namespaceMappings);
					ms.Position = 0;
					return (new StreamReader(ms).ReadToEnd());
				}
			}
		}

		public static void Serialize(object source, Stream stream, params NamespaceMapping[] namespaceMappings) {
			using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8)) {
				XmlFragmentWriter tw = new XmlFragmentWriter(writer) { Formatting = Formatting.Indented, IndentChar = '\t', Indentation = 1 };
				System.Xml.Serialization.XmlSerializer s = new System.Xml.Serialization.XmlSerializer(source.GetType());
				if (namespaceMappings != null) {
					XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
					foreach (NamespaceMapping nsm in namespaceMappings) {
						ns.Add(nsm.Prefix, nsm.Namespace);
					}
					s.Serialize(tw, source, ns);
				} else {
					s.Serialize(tw, source);
				}
			}
		}

		public static XmlElement SerializeAsElement(object source, params NamespaceMapping[] namespaceMappings) {
			string xml = Serialize(source, namespaceMappings);
			XmlDocument document = new XmlDocument();
			document.LoadXml(xml);
			return (document.DocumentElement);
		}

		public static TType Deserialize<TType>(string xml) {
			TType p;
			using (StringReader sr = new StringReader(xml)) {
				System.Xml.Serialization.XmlSerializer s = new System.Xml.Serialization.XmlSerializer(typeof (TType));
				p = (TType) s.Deserialize(sr);
			}
			return (p);
		}

		public static TType Deserialize<TType>(Stream stream) {
			TType p;
			using (StreamReader sr = new StreamReader(stream)) {
				System.Xml.Serialization.XmlSerializer s = new System.Xml.Serialization.XmlSerializer(typeof (TType));
				p = (TType) s.Deserialize(sr);
			}
			return (p);
		}

		public static object Deserialize(Type type, string xml) {
			using (StringReader sr = new StringReader(xml)) {
				System.Xml.Serialization.XmlSerializer s = new System.Xml.Serialization.XmlSerializer(type);
				return (s.Deserialize(sr));
			}
		}

	}

}
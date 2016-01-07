using System;
using System.Xml;
using Newtonsoft.Json;

namespace Utilities.WebApi {

	public class XmlElementJsonConverter : JsonConverter {

		public override bool CanWrite {
			get { return true; }
		}

		public override bool CanConvert(Type objectType) {
			return (objectType == typeof (XmlElement));
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
			string xml = reader.Value as String;
			XmlDocument xmlDoc = new XmlDocument();
			if (xml != null) {
				xmlDoc.LoadXml(xml);
			}
			return xmlDoc.DocumentElement;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
			XmlElement xmlElement = value as XmlElement;
			writer.WriteValue(xmlElement.OuterXml);
		}

	}

}
using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Utilities.WebApi {

	public static class JsonSerializerFactory {

		public const string _garblePrefix = ")]}',\n";

		public static string SerializeObject(this JsonSerializer serializer, object o) {
			using (StringWriter sw = new StringWriter()) {
				serializer.Serialize(sw, o);
				return sw.ToString();
			}
		}

		public static T DeSerializeObject<T>(this JsonSerializer serializer, string value, bool deGarble = false) {
			using (StringReader stringReader = new StringReader(value)) {
				if (deGarble && String.Compare(value.Substring(0, _garblePrefix.Length), _garblePrefix) == 0) {
					//read prefix to advance read pointer
					char[] buffer = new char[_garblePrefix.Length];
					stringReader.ReadBlock(buffer, 0, _garblePrefix.Length);
				}
				return serializer.Deserialize<T>(new JsonTextReader(stringReader));
			}
		}

		public static JsonSerializer Create() {
			JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver(), Formatting = Formatting.Indented, DateFormatHandling = DateFormatHandling.IsoDateFormat });
			serializer.Converters.Add(new XmlElementJsonConverter());
			return serializer;
		}

	}

}
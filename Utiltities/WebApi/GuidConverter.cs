using System;
using Newtonsoft.Json;

namespace Utilities.WebApi {

	public class GuidConverter : JsonConverter {

		public override bool CanRead {
			get { return false; }
		}

		public override bool CanWrite {
			get { return true; }
		}

		public override bool CanConvert(Type objectType) {
			return objectType == typeof (Guid);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
			Guid guid = (Guid) value;
			writer.WriteValue(guid.ToString("N").ToUpper());
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
			// the default deserialization works fine, 
			// but otherwise we'd handle that here
			throw new NotImplementedException();
		}

	}

}
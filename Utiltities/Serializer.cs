using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Utilities.WebApi;

namespace Utilities {

	public class Serializer {

		private static readonly JsonSerializerSettings settings;

		static Serializer() {
			settings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, ContractResolver = new CamelCasePropertyNamesContractResolver() };
			settings.Converters.Add(new GuidConverter());
		}

		public static string Serialize(object thing) {
			return (JsonConvert.SerializeObject(thing, Formatting.None, settings));
		}

		public static TType Deserialize<TType>(string source) {
			return (JsonConvert.DeserializeObject<TType>(source, settings));
		}

	}

}
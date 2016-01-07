using System.Linq;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Utilities.WebApi;
using JsonMediaTypeFormatter = System.Net.Http.Formatting.JsonMediaTypeFormatter;

namespace Server.Configuration {

	public class WebApiConfig {

		public static void Configure(HttpConfiguration config) {
			JsonMediaTypeFormatter formatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
			formatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
			formatter.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
			formatter.SerializerSettings.Formatting = Formatting.Indented;
			formatter.SerializerSettings.Converters.Add(new GuidConverter());
			formatter.SerializerSettings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
			config.MapHttpAttributeRoutes();
		}

	}

}
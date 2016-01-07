using System;
using System.Net.Http.Headers;

namespace Utilities.WebApi {

	public class JsonMediaTypeFormatter : BaseJsonMediaTypeFormatter {

		public JsonMediaTypeFormatter() {
			this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
		}

		public override bool CanReadType(Type type) {
			return true;
		}

		public override bool CanWriteType(Type type) {
			return true;
		}

	}

}
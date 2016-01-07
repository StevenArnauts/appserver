using System;
using System.Net.Http;

namespace Utilities.WebApi {

	public static class HTTPClientFactory {

		public static IHTTPClient Create(Uri serverUrl, HttpMessageHandler innerHandler = null) {
			IHTTPClient client = new HTTPClient(serverUrl);
			return ( client );
		}

	}

}
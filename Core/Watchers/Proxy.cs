using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Utilities.WebApi;

namespace Core {

	public class Proxy {

		private readonly HTTPClient _httpClient;

		public Proxy(Uri uri) {
			ApiMessageHandler apiMessageHandler = new ApiMessageHandler(new WebRequestHandler { UseCookies = false });
			this._httpClient = new HTTPClient(uri, apiMessageHandler);
		}

		public Update Check(string application, string currentVersion) {
			List<Package> updates = this._httpClient.Get<IEnumerable<Package>>("/api/applications/" + application + "/packages?since=" + currentVersion).ToList();
			if (!updates.Any()) return (null);
			Package latest = updates.Last();
			return (new Update { Application = application, Number = latest.Version, Timestamp = latest.Timestamp });
		}

		public byte[] Download(Update version) {
			string url = DownloadUrl(version);
			byte[] bytes = this._httpClient.Download(url);
			return (bytes);
		}

		public static string DownloadUrl(Update version) {
			string url = "/api/applications/" + version.Application + "/packages/" + version.Number + "/bytes";
			return url;
		}

		internal class Package {

			public string Version { get; set; }
			public DateTime Timestamp { get; set; }

		}

	}

}

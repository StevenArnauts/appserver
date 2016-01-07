using System;

namespace Utilities.WebApi {

	public interface IHTTPClient : IDisposable {

		string Get(string path);
		string Get(Uri uri);
		TResponse Get<TResponse>(string path);
		TResponse Get<TResponse>(Uri uri);
		TResponse Post<TRequest, TResponse>(string path, TRequest content);
		byte[] Download(string url);
		void Download(string url, string localPath);

	}

}
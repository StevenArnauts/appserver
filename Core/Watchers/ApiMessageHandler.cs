using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Core {

	public class ApiMessageHandler : DelegatingHandler {

		public ApiMessageHandler(HttpMessageHandler innerHandler) {
			this.InnerHandler = innerHandler;
		}

		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
			request.Headers.Remove("Accept");
			request.Headers.Add("Accept", "application/json");
			return (base.SendAsync(request, cancellationToken));
		}

	}

}
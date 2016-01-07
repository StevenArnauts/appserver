using System;
using System.Net;
using System.Net.Http;

namespace Utilities.WebApi {

	public class DefaultErrorTranslator : BaseTranslator {

		protected override HttpResponseMessage InternalTranslate(Exception exception, HttpRequestMessage request) {
			if(exception is InvalidOperationException) return request.CreateResponse(HttpStatusCode.BadRequest, new Error { Message = exception.Message });
			if(exception is ObjectNotFoundException) return request.CreateResponse(HttpStatusCode.NotFound, new Error { Message = exception.Message });
			return ( null );
		}

	}

}
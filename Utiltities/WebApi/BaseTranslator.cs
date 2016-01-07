using System;
using System.Net;
using System.Net.Http;

namespace Utilities.WebApi {

	public abstract class BaseTranslator : IErrorTranslator {

		public IErrorTranslator InnerTranslator { get; set; }

		protected abstract HttpResponseMessage InternalTranslate(Exception exception, HttpRequestMessage request);

		public HttpResponseMessage Translate(Exception exception, HttpRequestMessage request) {
			HttpResponseMessage result = this.InternalTranslate(exception, request);
			if(result == null) if(this.InnerTranslator != null) result = this.InnerTranslator.Translate(exception, request);
			if(result == null) result = new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(Logger.FormatException(exception)) };
			return ( result );
		}

	}

}
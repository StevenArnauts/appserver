using System;
using System.Net.Http;

namespace Utilities.WebApi {

	public interface IErrorTranslator {

		HttpResponseMessage Translate(Exception exception, HttpRequestMessage request);

		/// <summary>
		/// This is a way to extend the translator: if it doesn't know what to do with the exception, it delegates it to this handler
		/// </summary>
		IErrorTranslator InnerTranslator { get; set; }

	}

}
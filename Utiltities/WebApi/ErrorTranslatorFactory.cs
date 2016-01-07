namespace Utilities.WebApi {

	public static class ErrorTranslatorFactory {

		private static IErrorTranslator translator;

		static ErrorTranslatorFactory() {
			Translator = new DefaultErrorTranslator();
		}

		/// <summary>
		/// Allows to replace the default error translation handling with something custom.
		/// </summary>
		public static IErrorTranslator Translator {
			get { return translator; }
			set {
				translator = value;
			}
		}

	}

}
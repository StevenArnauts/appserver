using System;

namespace Core {

	public class AppServerException : ApplicationException {

		public AppServerException(string message) : base(message) {}
		public AppServerException(string message, Exception inner) : base(message, inner) {}

	}

}
namespace Utilities.WebApi {

	public class Error {

		public const int HTTP_STATUS_CODE_TOKEN_EXPIRED = 419;

		public const int TOKEN_EXPIRED = 100;
		public const int USER_LOCKED = 101;
		public const int USER_BLOCKED = 102;


		public string Message { get; set; }
		public int Code { get; set; }

	}

}
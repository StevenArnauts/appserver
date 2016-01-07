using System;

namespace Utilities {

	public class SystemDateTimeFactory : IDateTimeFactory {
		public DateTime GetNow() {
			return DateTime.Now;
		}

		public DateTime GetUtcNow() {
			return DateTime.UtcNow;
		}
	}

}
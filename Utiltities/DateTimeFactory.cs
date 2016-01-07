using System;

namespace Utilities {

	public static class DateTimeFactory {

		public static DateTime Now {
			get {
				return Dependency.Resolve<IDateTimeFactory>().GetNow();
			}
		}

		public static DateTime UtcNow {
			get {
				return Dependency.Resolve<IDateTimeFactory>().GetUtcNow();
			}
		}

		public static DateTimeOffset CurrentOffset {
			get {
				var timeStamp = Dependency.Resolve<IDateTimeFactory>().GetNow();
				return new DateTimeOffset(timeStamp, TimeZone.CurrentTimeZone.GetUtcOffset(timeStamp));
			}
		}

	}

}
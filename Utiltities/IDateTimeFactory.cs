using System;

namespace Utilities {

	public interface IDateTimeFactory {
		DateTime GetNow();
		DateTime GetUtcNow();
	}

}
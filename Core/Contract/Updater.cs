using System;
using Core.Contract;

namespace Core {

	/// <summary>
	/// Types that implement interface are run after the deployment of a package.
	/// </summary>
	public abstract class Updater : MarshalByRefObject {

		public abstract void Run(Context context);

	}

}
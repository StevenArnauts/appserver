using System;
using Utilities;

namespace Alure {

	public class AlureCallback : MarshalByRefObject {

		public override object InitializeLifetimeService() {
			return (null);
		}

		public void Call(string message) {
			Logger.Info(this, "Received callback message " + message);
		}

	}

}
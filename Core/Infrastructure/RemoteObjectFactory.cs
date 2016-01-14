using System;

namespace Core {

	public static class RemoteObjectFactory {

		private static readonly SponsorshipManager sponsorshipManager = new SponsorshipManager();

		public static SponsorshipManager SponsorshipManager {
			get { return (sponsorshipManager); }
		}

		/// <summary>
		/// Creates an <see cref="System.Runtime.Remoting.Lifetime.ISponsor"/> for the target, which will keep the remote object connected and safe from garbage collection.
		/// </summary>
		/// <param name="target"></param>
		public static void Sponsor(MarshalByRefObject target) {
			sponsorshipManager.Register(target);
		}

	}

}
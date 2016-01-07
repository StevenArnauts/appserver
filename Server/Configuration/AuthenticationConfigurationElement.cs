using System;
using System.Configuration;
using Utilities;

namespace Server {

	[Serializable]
	public class AuthenticationConfigurationElement : ConfigurationElementBase, IAuthenticationConfiguration {

		private const string KEY_USER_ID = "userid";
		private const string KEY_PASSWORD = "password";

		[ConfigurationProperty(KEY_USER_ID, IsRequired = true, IsKey = false)]
		public string UserId {
			get { return ( this.GetValue<string>(KEY_USER_ID) ); }
			set { this[KEY_USER_ID] = value; }
		}

		[ConfigurationProperty(KEY_PASSWORD, IsRequired = true, IsKey = false)]
		public string Password {
			get { return ( this.GetValue<string>(KEY_PASSWORD) ); }
			set { this[KEY_PASSWORD] = value; }
		}

	}

}
using System;
using System.Configuration;
using Utilities;

namespace Server {

	[Serializable]
	public class ServiceConfigurationElement : ConfigurationElementBase, IServiceConfiguration {

		private const string KEY_PORT = "port";

		[ConfigurationProperty(KEY_PORT, IsRequired = true, IsKey = false)]
		public int Port {
			get { return (this.GetValue<int>(KEY_PORT)); }
			set { this[KEY_PORT] = value; }
		}

	}

}
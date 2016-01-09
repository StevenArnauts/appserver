using System;
using System.Configuration;
using Server.Persistence;
using Utilities;

namespace Server {

	[Serializable]
	public class ApplicationRepositoryConfigurationElement : ConfigurationElementBase, IApplicationRepositoryConfiguration {

		private const string KEY_ROOT_FOLDER = "rootFolder";
		private const string KEY_TEMP_FOLDER = "tempFolder";

		[ConfigurationProperty(KEY_ROOT_FOLDER, IsRequired = true, IsKey = false)]
		public string RootFolder {
			get { return ( this.GetValue<string>(KEY_ROOT_FOLDER) ); }
			set { this[KEY_ROOT_FOLDER] = value; }
		}

		[ConfigurationProperty(KEY_TEMP_FOLDER, IsRequired = true, IsKey = false)]
		public string TempFolder {
			get { return ( this.GetValue<string>(KEY_TEMP_FOLDER) ); }
			set { this[KEY_TEMP_FOLDER] = value; }
		}

	}

}
using System.IO;
using Core;
using Core.Contract;
using Utilities;

namespace Alure {

	public class ConfigUpdate : Updater {

		public override void Run(ServerContext context) {
			Logger.Info(this, "Updating...");
			string configFilePath = context.ConfigFilePath;
			string transformationFilePath = Path.Combine(context.CurrentPath, "add_user_sex.xdt");
			FileTransformer transformer = new FileTransformer(configFilePath, configFilePath, transformationFilePath);
			transformer.Run();
			Logger.Info(this, "Done");
		}

	}

}
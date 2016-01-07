using System;
using System.IO;
using Utilities;

namespace Core.Contract {

	/// <summary>
	/// Mechanism to allow the host application to receive messages from the guest applications.
	/// This object always lives in the app domain of the server.
	/// </summary>
	public class Context : MarshalByRefObject {

		private readonly Application _application;
		private readonly Server _server;

		public Context(Application application, Server server) {
			this._server = server;
			this._application = application;
			Logger.Info(this, "Context created in app domain " + AppDomain.CurrentDomain.FriendlyName);
		}

		/// <summary>
		/// Returns a random temp path in the server's temp folder.
		/// </summary>
		/// <returns></returns>
		public string GetTempPath() {
			return ( Path.Combine(this._server.TempFolder, Guid.NewGuid().ToString("N").ToUpper()) );
		}

		/// <summary>
		/// Gets the path to the applications user settings file
		/// </summary>
		public string ConfigFilePath {
			get { return (this._application.SettingsPath); }
		}

		/// <summary>
		/// Gets the path where the application is currently executing from
		/// </summary>
		public string CurrentPath {
			get { return (this._application.BinFolder); }
		}

		/// <summary>
		/// Gets the base folder where applications are installed on the server
		/// </summary>
		public string AppFolder {
			get { return (this._server.AppFolder); }
		}

		public string HandleMessage(Message message) {
			Logger.Debug(this, "Received message " + message.Id + " : " + message.Content);
			return ( message.Id );
		}

	}

}
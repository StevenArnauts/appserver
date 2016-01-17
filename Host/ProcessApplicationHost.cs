using System;
using System.Diagnostics;
using System.IO;
using Core.Contract;
using Utilities;
using Type = Core.Contract.Type;

namespace Core.ProcessHost {

	internal class ProcessApplicationHost : IApplicationHost, IDisposable {

		private readonly string _binFolder;
		private Process _hostProcess;
		private string _url;

		public ProcessApplicationHost(string binFolder, int port) {
			this._binFolder = binFolder;
			this.LaunchHost(port);
		}

		private IHost Connection {
			get {
				IHost host = (IHost) Activator.GetObject(typeof (IHost), this._url);
				Logger.Info("Connecting to " + this._url + "...");
				return host;
			}
		}

		public void Initialize(ServerContext context, Type bootstrapper) {
			try {
				this.Connection.Initialize(bootstrapper, context);
			} catch (Exception ex) {
				Logger.Error(ex, "Failed to initialize host process");
			}
		}

		public void Start() {
			try {
				Logger.Info("Sending start command to host...");
				this.Connection.Start();
				Logger.Info("Start command sent");
			} catch (Exception ex) {
				Logger.Error(ex, "Failed to start host process");
			}
		}

		public void Stop() {
			try {
				Logger.Info("Sending stop command to host...");
				this.Connection.Stop();
				Logger.Info("Stop command sent");
			} catch (Exception ex) {
				Logger.Error(ex, "Failed to stop host process");
			}
		}

		public void Destroy() {
			Logger.Info("Destroying host process...");
			try {
				this._hostProcess.Kill();
				Logger.Info("Waiting for host process to exit...");
				this._hostProcess.WaitForExit(10000);
				Logger.Info("Hosting stopped");
				this._hostProcess.Dispose();
			} catch (Exception ex) {
				Logger.Error(ex, "Failed to destroy host process");
			}
		}

		public void Dispose() {
			this.Destroy();
		}

		private void LaunchHost(int port) {

			// find the apphost.exe and copy it to the target location, overwrite if needed so we have the correct version!
			string assemblyFile = Path.GetFileName(new Uri(typeof (Server).Assembly.CodeBase).LocalPath);
			string source = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assemblyFile);
			string target = Path.Combine(this._binFolder, assemblyFile);
			File.Copy(source, target, true);
			Logger.Info("Copied " + source + " to " + target);

			this._url = "tcp://localhost:" + port + "/" + Server.OBJECT_URI;

			// now start it as a new process
			this._hostProcess = new Process { EnableRaisingEvents = true, StartInfo = new ProcessStartInfo { Arguments = port.ToString(), CreateNoWindow = false, FileName = target } };
			this._hostProcess.Start();
			Logger.Info("Started new application host process on port " + port);
		}

	}

}
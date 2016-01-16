//using System;
//using System.Diagnostics;
//using System.IO;
//using System.Reflection;
//using System.Runtime.Remoting.Channels;
//using System.Runtime.Remoting.Channels.Tcp;
//using System.Threading;
//using Core.Contract;
//using Core.Infrastructure;
//using Utilities;

//namespace Core {

//	internal class ProcessApplicationHost : ApplicationHost, IDisposable {

//		private readonly string _assembly;
//		private readonly string _binFolder;
//		private Process _hostProcess;
//		private string _url;

//		public ProcessApplicationHost(string binFolder, string assembly) {
//			this._assembly = assembly;
//			this._binFolder = binFolder;
//			this.LaunchHost();
//		}

//		public void Dispose() {
//			this.Destroy();
//		}

//		private void LaunchHost() {

//			// find the apphost.exe and copy it to the target location, overwrite if needed so we have the correct version!
//			string assemblyFile = Path.GetFileName(new Uri(typeof (Host.Server).Assembly.CodeBase).LocalPath);
//			string source = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assemblyFile);
//			string target = Path.Combine(this._binFolder, assemblyFile);
//			File.Copy(source, target, true);
//			Logger.Info(this, "Copied " + source + " to " + target);

//			// find a free port for the host to use
//			int port = IPHelper.FindFreePort(8000, 9000);
//			if(port < 0) throw new AppServerException("Unable to find a free port for the application hosting process");
//			this._url = "tcp://localhost:" + port + "/" + Host.Server.OBJECT_URI;

//			// now start it as a new process
//			this._hostProcess = new Process { EnableRaisingEvents = true, StartInfo = new ProcessStartInfo { Arguments = port.ToString(), CreateNoWindow = false, FileName = target } };
//			this._hostProcess.Start();
//			Logger.Info(this, "Started new application host process on port " + port);

//		}

//		public void Initialize(Context context, Contract.Type bootstrapper) {
//			TcpChannel chan = new TcpChannel();
//			ChannelServices.RegisterChannel(chan, false);
//			MethodInfo initializeMethod = SymbolExtensions.GetMethodInfo<IBootstrapper>(b => b.Initialize(null));
//			this.Connection.Initialize(bootstrapper, context);
//		}

//		internal override void Start() {
//			Logger.Info("Sending start command to host...");
//			MethodInfo startMethod = SymbolExtensions.GetMethodInfo<IBootstrapper>(b => b.Run(default(CancellationToken)));
//			this.Connection.Start();
//			Logger.Info(this, "Start command sent");
//		}

//		internal override void Stop() {
//			Logger.Info("Sending stop command to host...");
//			this.Connection.Stop();
//			Logger.Info(this, "Stop command sent");
//		}

//		internal override void Destroy() {
//			Logger.Info(this, "Destroying host process...");
//			this._hostProcess.Kill();
//			Logger.Info(this, "Waiting for host process to exit...");
//			this._hostProcess.WaitForExit(10000);
//			Logger.Info(this, "Hosting stopped");
//			this._hostProcess.Dispose();
//		}

//		private IHost Connection {
//			get {
//				IHost host = (IHost)Activator.GetObject(typeof(IHost), this._url);
//				Logger.Info(this, "Connecting to " + this._url + "...");
//				return host;
//			}
		
//		}

//	}

//}
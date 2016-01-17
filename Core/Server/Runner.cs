using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Contract;
using Core.Infrastructure;
using Utilities;

namespace Core {

	/// <summary>
	/// This is the connection between the server and the application's appdomain. It's created by the server when the application starts
	/// IN the application's own app domain. The <see cref="Runner"/> will then instantiate the applications <see cref="IBootstrapper"/>
	/// and manage it.
	/// </summary>
	public class Runner : RemoteObject {

		private IBootstrapper _bootstrapper;
		private CancellationTokenSource _cancellationTokenSource;
		private Task _task;

		/// <summary>
		/// Creates the <see cref="Contract.IBootstrapper"/> instance.
		/// </summary>
		public void Initialize(ServerContext context, Contract.Type bootstrapperType, string[] args) {
			Logger.Initialize("log4net.config");
			Logger.Info(this, "Initializing " + bootstrapperType.FullName + "...");
			this._bootstrapper = ReflectionHelper.CreateInstance<IBootstrapper>(AppDomain.CurrentDomain, bootstrapperType);
			this._bootstrapper.Initialize(context, args);
			Logger.Info(this, "Initialized " + bootstrapperType.FullName);
		}

		/// <summary>
		/// Starts the <see cref="IBootstrapper"/>
		/// </summary>
		public void Start() {
			Logger.Info(this, "Starting bootstrapper " + this._bootstrapper.GetType().FullName + "...");
			this._cancellationTokenSource = new CancellationTokenSource();
			CancellationToken token = this._cancellationTokenSource.Token;
			this._task = new Task(() => this._bootstrapper.Run(token));
			this._task.Start();
			Logger.Info(this, "Bootstrapper started");
		}

		/// <summary>
		/// Stops the <see cref="IBootstrapper"/>
		/// </summary>
		public void Stop() {
			Logger.Info(this, "Stopping " + this._bootstrapper.GetType().FullName + "...");
			this._cancellationTokenSource.Cancel(false);
			Logger.Info(this, "Waiting for bootstrapper " + this._bootstrapper.GetType().FullName + " to finish...");
			this._task.Wait();
			Logger.Info(this, "Bootstrapper stopped");
		}

	}

}
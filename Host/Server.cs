using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Core.Contract;
using Core.Infrastructure;
using Utilities;

namespace Core.ProcessHost {

	public class Server : MarshalByRefObject, IHost {

		private object _bootstrapper;
		private CancellationTokenSource _cancellationTokenSource;
		private Task _task;

		public const string OBJECT_URI = "AppHost";

		public Server() {
			Logger.Info("Created");
		}

		public override object InitializeLifetimeService() { return (null); }

		public void Initialize(Contract.Type bootstrapper, ServerContext context) {
			Logger.Info("Initializing bootstrapper " + bootstrapper.AssemblyQualifiedName + "...");
			this._bootstrapper = this.CreateInstance(bootstrapper.AssemblyQualifiedName);
			SymbolExtensions.GetMethodInfo<IBootstrapper>(b => b.Initialize(null)).Invoke(this._bootstrapper, new object[] {context});
			Logger.Info("Initialized");
		}

		public void Start() {
			Logger.Info("Starting...");
			this._cancellationTokenSource = new CancellationTokenSource();
			CancellationToken token = this._cancellationTokenSource.Token;
			this._task = new Task(() => SymbolExtensions.GetMethodInfo<IBootstrapper>(b => b.Run(default(CancellationToken))).Invoke(this._bootstrapper, new object[] {token}));
			this._task.Start();
			Logger.Info("Bootstrapper started");
		}

		public void Stop() {
			Logger.Info("Stopping...");
			this._cancellationTokenSource.Cancel(false);
			Logger.Info("Waiting for bootstrapper " + this._bootstrapper.GetType().FullName + " to finish...");
			this._task.Wait();
			Logger.Info("Bootstrapper stopped");
		}

		private object CreateInstance(string typeName) {
			System.Type bootstrapperType = System.Type.GetType(typeName, true);
			ConstructorInfo constructor = bootstrapperType.GetConstructor(System.Type.EmptyTypes);
			if(constructor == null) throw new Exception("Type " + typeName + " does not have a default constructor");
			object instance = constructor.Invoke(null);
			Logger.Info("Created bootstrapper instance of " + typeName);
			return (instance);
		}

	}

}
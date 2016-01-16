using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Host {

	public class Server : MarshalByRefObject {

		private object _bootstrapper;
		private CancellationTokenSource _cancellationTokenSource;
		private Task _task;

		public const string OBJECT_URI = "AppHost";

		public Server() {
			Logger.Log("Created");
		}

		public override object InitializeLifetimeService() { return (null); }

		public void Start(MethodInfo startMethod) {
			Logger.Log("Starting...");
			this._cancellationTokenSource = new CancellationTokenSource();
			CancellationToken token = this._cancellationTokenSource.Token;
			this._task = new Task(() => startMethod.Invoke(this._bootstrapper, new object[] {token}));
			this._task.Start();
			Logger.Log("Bootstrapper started");
		}

		public void Stop() {
			Logger.Log("Stopping...");
			this._cancellationTokenSource.Cancel(false);
			Logger.Log("Waiting for bootstrapper " + this._bootstrapper.GetType().FullName + " to finish...");
			this._task.Wait();
			Logger.Log("Bootstrapper stopped");
		}

		public void Initialize(string bootstrapperTypeName, MethodInfo initializeMethod) {
			Logger.Log("Initializing bootstrapper " + bootstrapperTypeName + "...");
			this._bootstrapper = this.CreateInstance(bootstrapperTypeName);
			initializeMethod.Invoke(this._bootstrapper, new object[] {null});
			Logger.Log("Initialized");
		}

		private object CreateInstance(string typeName) {
			Type bootstrapperType = Type.GetType(typeName, true);
			ConstructorInfo constructor = bootstrapperType.GetConstructor(Type.EmptyTypes);
			if(constructor == null) throw new Exception("Type " + typeName + " does not have a default constructor");
			object instance = constructor.Invoke(null);
			Logger.Log("Created bootstrapper instance of " + typeName);
			return (instance);
		}

	}

}
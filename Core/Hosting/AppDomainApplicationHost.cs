using System;
using Core.Contract;
using Core.Infrastructure;
using Utilities;

namespace Core {

	internal class AppDomainApplicationHost : ApplicationHost {

		private AppDomain _appDomain;
		private Runner _runner;

		public AppDomainApplicationHost(string binFolder, string assembly) {
			this._appDomain = ReflectionHelper.LoadAppDomain(binFolder, assembly);
			this._runner = ReflectionHelper.CreateInstance<Runner>(this._appDomain, Type.FromType(typeof(Runner)));
		}

		//public AppDomain AppDomain {
		//	get { return this._appDomain; }
		//	set { this._appDomain = value; }
		//}

		//public Runner Runner {
		//	get { return this._runner; }
		//	set { this._runner = value; }
		//}

		//internal override TInstance CreateInstance<TInstance>(Type type) {
		//	string file = type.Assembly.File;
		//	if (file == null) throw new ArgumentException("Type must include assembly location");
		//	object proxy = RemoteObjectFactory.Create(this._appDomain, type, null);
		//	TInstance instance = proxy as TInstance;
		//	if (instance == null) throw new Exception("Type " + type.FullName + " cannot be cast to " + typeof (TInstance).FullName);
		//	Logger.Info(this, "Created instance of " + type.FullName + " from " + file + " in app domain " + this._appDomain.FriendlyName);
		//	return (instance);
		//}

		internal override void Initialize(Context context, Type bootstrapper) {
			this._runner.Initialize(context, bootstrapper);
		}

		internal override void Start() {
			this._runner.Start();
		}

		internal override void Stop() {
			this._runner.Stop();
		}

		internal override void Destroy() {
			int attempt = 0;
			const int attempts = 10;
			while(attempt <= attempts) {
				try {
					attempt++;
					Logger.Info(this, "Unloading, attempt " + attempt + " of " + attempts + "...");
					AppDomain.Unload(this._appDomain);
					break;
				} catch(CannotUnloadAppDomainException) {
					Logger.Warn(this, "Failed to stop in time because the appdomain could not be unloaded");
				} catch(Exception e) {
					Logger.Error(this, e, "Failed to stop.");
					break;
				}
			}
			this._appDomain = null;
			this._runner = null;
		}

	}

}
using System;
using Core.Contract;
using Core.Infrastructure;
using Utilities;
using Type = Core.Contract.Type;

namespace Core {

	internal class AppDomainApplicationHost : IApplicationHost {

		private AppDomain _appDomain;
		private Runner _runner;

		public AppDomainApplicationHost(string binFolder, string assembly) {
			this._appDomain = ReflectionHelper.LoadAppDomain(binFolder, assembly);
			this._runner = ReflectionHelper.CreateInstance<Runner>(this._appDomain, Type.FromType(typeof(Runner)));
		}

		public void Initialize(ServerContext context, Type bootstrapper, string[] args) {
			this._runner.Initialize(context, bootstrapper, args);
		}

		public void Start() {
			this._runner.Start();
		}

		public void Stop() {
			this._runner.Stop();
		}

		public void Destroy() {
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
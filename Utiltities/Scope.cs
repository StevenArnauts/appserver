using System;
using Autofac;

namespace Utilities {

	/// <summary>
	/// Nested container designed to override registered services in a bounded context like a using block, 
	/// mostly usefull in unit tests.
	/// </summary>
	public class Scope : IDisposable {

		private readonly object _lock = new object();
		private readonly Action<Scope> _action;
		private readonly ILifetimeScope _scope;
		private ContainerBuilder _builder;
		private bool _isDisposed;

		public Scope(IContainer container, Action<Scope> action) {
			this._action = action;
			this._scope = container.BeginLifetimeScope(this.Build);
			Logger.Debug(this, "Created " + this.GetType().Name + " " + this.GetHashCode());
		}

		public void Register<T>(T implementation) where T : class {
			this._builder.Register(x => implementation);
		}

		public void RegisterType(Type type) {
			this._builder.RegisterType(type);
		}

		public void RegisterInstance(object instance) {
			this._builder.RegisterInstance(instance);
		}

		public T Resolve<T>() where T : class {
			return (this._scope.Resolve<T>());
		}

		public object Resolve(Type type) {
			return (this._scope.Resolve(type));
		}

		private void Build(ContainerBuilder builder) {
			this._builder = builder;
			this._action(this);
		}

		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing) {
			Logger.Debug(this, "Disposing " + this.GetType().Name + " " + this.GetHashCode() + "...");
			lock (this._lock) {
				if(!this._isDisposed) {
					if(disposing) {
						this._scope.Dispose();
					}
					this._isDisposed = true;
					Logger.Debug(this, "Disposed " + this.GetType().Name + " " + this.GetHashCode() + "...");
				} else {
					Logger.Warn(this.GetType().Name + " " + this.GetHashCode() + " is already disposed");
				}
			}
		}

		~Scope() {
			this.Dispose(false);
		}

	}

}
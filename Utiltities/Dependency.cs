using System;
using Autofac;

namespace Utilities {

	/// <summary>
	/// Wrapper for the DI container of choice (currently Autofac)
	/// </summary>
	public class Dependency {

		private static IContainer container;

		public static T Resolve<T>() where T : class {
			return (container.Resolve<T>());
		}

		public static object Resolve(Type type) {
			return (container.Resolve(type));
		}

		public static IContainer Container {
			get { return (container); }
		}

		public static IContainer Build(ContainerBuilder builder) {
			container = builder.Build();
			return (container);
		}

		/// <summary>
		/// Updates the container with the new builder information
		/// </summary>
		/// <param name="builder"></param>
		public static void Update(ContainerBuilder builder) {
			builder.Update(Container);
		}

		public static Scope StartScope(Action<Scope> action) {
			Scope scope = new Scope(container, action);
			return (scope);
		}

	}

}
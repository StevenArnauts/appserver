using System;
using Utilities;

namespace Core.Contract {

	[Serializable]
	public class Type {

		public static Type FromType(System.Type source) {
			Type name = new Type {
				FullName = source.FullName, 
				Name = source.Name, 
				AssemblyQualifiedName = source.AssemblyQualifiedName,
				Assembly = new Assembly {
					File = source.Assembly.Path(),
					Name = source.Assembly.Name(), 
					Version = source.Assembly.Version()
				}
			};
			return (name);
		}

		public System.Type AsType() {
			System.Type type = System.Type.GetType(this.AssemblyQualifiedName);
			return (type);
		}

		public string AssemblyQualifiedName { get; private set; }
		public string Name { get; private set; }
		public string FullName { get; private set; }
		public Assembly Assembly { get; private set; }

	}

}
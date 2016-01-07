using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Contract {

	[Serializable]
	public class Package {

		private readonly List<Type> _updaters = new List<Type>();
		private readonly List<Type> _bootstrappers = new List<Type>();

		public Manifest Manifest { get; internal set; }

		public ICollection<Type> Bootstrappers {
			get { return this._bootstrappers; }
		}

		public ICollection<Type> Updaters {
			get { return this._updaters; }
		}

		public Type Bootstrapper {
			get {
				if(!this._bootstrappers.Any()) throw new InvalidOperationException("Package contains no bootstrapper");
				if(this._bootstrappers.Count > 1) throw new InvalidOperationException("Package contains " + this._bootstrappers.Count + " bootstrapper(s)");
				return(this._bootstrappers.First());
			}
		}

	}

}
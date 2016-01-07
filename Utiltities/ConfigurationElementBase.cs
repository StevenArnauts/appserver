using System.Collections.Generic;
using System.Configuration;

namespace Utilities {

	public abstract class ConfigurationElementBase : ConfigurationElement {

		private readonly object _lock = new object();
		private readonly Dictionary<string, object> _propertyCache = new Dictionary<string, object>();

		protected T GetValue<T>(string key, T @default = default(T)) {
			lock (this._lock) {
				if(this._propertyCache.ContainsKey(key)) {
					return ((T)this._propertyCache[key]);
				}
				T value = this.DetermineValue(key, @default);
				this._propertyCache.Add(key, value);
				return (value);
			}
		}

		private T DetermineValue<T>(string name, T dflt) {
			T val = (T)this[name];
			if(!this.IsElementSpecified(name)) {
				val = dflt;
			} 
			return (val);
		}

		private bool IsElementSpecified(string name) {
			PropertyInformation p = this.ElementInformation.Properties[name];
			if(p != null) {
				return (p.ValueOrigin != PropertyValueOrigin.Default);
			}
			return (false);
		}

	}

}
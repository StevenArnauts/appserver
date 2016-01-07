using System.Configuration;

namespace Utilities {

	public abstract class ElementCollection<TConfigurationElement> : ConfigurationElementCollection where TConfigurationElement : ConfigurationElement {

		public object[] Keys {
			get { return (this.BaseGetAllKeys()); }
		}

		public new TConfigurationElement this[string key] {
			get { return ((TConfigurationElement)this.BaseGet(key)); }
			set {
				TConfigurationElement element = (TConfigurationElement)this.BaseGet(key);
				if(element != null) this.BaseRemove(key);
				this.BaseAdd(value);
			}
		}

		public void Add(TConfigurationElement configuration) {
			base.BaseAdd(configuration);
		}

		protected abstract TConfigurationElement CreateElement();
		protected abstract object CreateElementKey(TConfigurationElement element);

		protected override ConfigurationElement CreateNewElement() {
			return (this.CreateElement());
		}

		protected override object GetElementKey(ConfigurationElement element) {
			return (this.CreateElementKey((TConfigurationElement)element));
		}

	}

}
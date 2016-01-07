using System;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using System.Web.Http.ValueProviders;

namespace Utilities.WebApi {

	public class VersionModelBinder : IModelBinder {

		public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext) {
			if(bindingContext.ModelType != typeof(Version)) {
				return false;
			}

			ValueProviderResult val = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
			if(val == null) {
				return false;
			}

			string key = val.RawValue as string;
			if(key == null) {
				bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Wrong value type");
				return false;
			}

			Version result;
			if(Version.TryParse(key, out result)) {
				bindingContext.Model = result;
				return true;
			}

			bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Cannot convert value to Version");
			return false;
		}
	}

}
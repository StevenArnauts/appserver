using System.Text;
using System.Web.Mvc;

namespace Utilities.MVC {

	public class TraceAttribute : ActionFilterAttribute {

		public override void OnActionExecuting(ActionExecutingContext filterContext) {
			base.OnActionExecuting(filterContext);
			Logger.Debug(this, () => this.FormatAction(filterContext));
		}

		private string FormatAction(ActionExecutingContext filterContext) {
			StringBuilder builder = new StringBuilder();
			builder.Append("Action " + filterContext.ActionDescriptor.ControllerDescriptor.ControllerName + "." + filterContext.ActionDescriptor.ActionName + " called");
			builder.Append(" with arguments ");
			foreach (string key in filterContext.ActionParameters.Keys) {
				builder.Append(key + " = " + filterContext.ActionParameters[key] + " ");
			}
			builder.Append(", referrer = " + (filterContext.RequestContext.HttpContext.Request.UrlReferrer == null ? "none" : filterContext.RequestContext.HttpContext.Request.UrlReferrer.AbsoluteUri));
			return (builder.ToString());
		}

	}

}
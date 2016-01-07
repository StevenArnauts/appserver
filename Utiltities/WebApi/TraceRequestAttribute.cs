using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Utilities.WebApi {

	public class TraceRequestAttribute : ActionFilterAttribute {

		public override void OnActionExecuting(HttpActionContext actionContext) {
			Logger.Debug(actionContext.ActionDescriptor.ControllerDescriptor.ControllerType, actionContext.Request.RequestUri + " => " + actionContext.ActionDescriptor.ActionName + "(" + actionContext.ActionArguments.Print(Serializer.Serialize) + ")");
		}

		public override void OnActionExecuted(HttpActionExecutedContext context) {
			if(context.Exception != null) {
				Logger.Error(context.Exception, context.ActionContext.ControllerContext.Request.RequestUri.ToString());
			}
		}

	}

}
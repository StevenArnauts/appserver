using System.Web.Http.Filters;

namespace Utilities.WebApi {

	public class HandleErrorsAttribute : ActionFilterAttribute {

		public override void OnActionExecuted(HttpActionExecutedContext context) {
			if(context.Exception != null) {
				Logger.Error(context.Exception, context.ActionContext.ControllerContext.Request.RequestUri.ToString());
				context.Response = ErrorTranslatorFactory.Translator.Translate(context.Exception, context.Request);
			}
		}

	}

}
using System.Web.Mvc;
using Utilities;

namespace Server {

	public class BaseController : Controller {

		protected override void OnException(ExceptionContext filterContext) {
			filterContext.ExceptionHandled = true;
			Logger.Error(filterContext.Exception);
			this.TempData.Add("error", filterContext.Exception);
			// this.ViewData.Add("error", filterContext.Exception);
			filterContext.Result = this.RedirectToAction("Index", "Error");
		}

	}

}
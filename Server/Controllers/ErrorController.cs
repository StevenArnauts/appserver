using System;
using System.Web.Mvc;
using Utilities.MVC;

namespace Server {

	public class ErrorController : Controller {

		[Trace]
		[ActionName("Index")]
		[HttpGet]
		public ActionResult Index() {
			object o = this.TempData["error"];
			Exception error = o as Exception;
			this.TempData.Remove("error");
			ErrorModel model = new ErrorModel();
			if(error != null) {
				model.Type = error.GetType().FullName;
				model.Message = error.Message;
				model.StackTrace = error.StackTrace.Replace(Environment.NewLine, "<br/>");
			}
			return this.View(model);
		}

	}

}
using System.Web.Mvc;

namespace Server {

	public class HomeController : BaseController {

		public ActionResult Index() {
			return this.RedirectToAction("List", "Application");
		}

	}

}
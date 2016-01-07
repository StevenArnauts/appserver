using System.Web.Mvc;
using Core.Persistence;
using Utilities.MVC;

namespace Server {

	public class ApplicationController : BaseController {

		private readonly IApplicationRepository _repository;

		public ApplicationController(IApplicationRepository repository) {
			this._repository = repository;
		}

		[Trace]
		[ActionName("List")]
		[HttpGet]
		public ActionResult List() {
			ApplicationListModel model = new ApplicationListModel { Applications = this._repository.GetApplications() };
			return View("List", model);
		}

		[Authorize]
		[Trace]
		[ActionName("Add")]
		[HttpGet]
		public ActionResult Add() {
			return View("Add");
		}

		[Authorize]
		[Trace]
		[ActionName("Add")]
		[HttpPost]
		public ActionResult AddSubmit(ApplicationAddModel model) {
			this._repository.CreateApplication(model.Name);
			return RedirectToAction("List");
		}

	}

}
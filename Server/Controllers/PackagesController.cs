using System.Web.Mvc;
using Core;
using Server.Domain;
using Utilities;
using Utilities.MVC;
using Utilities.WebApi;

namespace Server {

	[HandleErrors]
	public class PackageController : BaseController {

		private readonly IPackageRepository _repository;

		public PackageController(IPackageRepository repository) {
			this._repository = repository;
		}

		[Trace]
		[ActionName("List")]
		[HttpGet]
		public ActionResult List(string application) {
			Domain.Application app = this._repository.GetApplication(application);
			PackageListModel model = new PackageListModel { Packages = this._repository.GetPackages(app), Application = application };
			return View("List", model);
		}

		[Trace]
		[ActionName("Add")]
		[HttpGet]
		public ActionResult Add(string application) {
			PackageAddModel model = new PackageAddModel();
			model.Application = application;
			return View("Add", model);
		}

		[Trace]
		[ActionName("Add")]
		[HttpPost]
		public ActionResult AddSubmit(PackageAddModel model) {
			if(model.File != null && model.File.ContentLength > 0) {
				Logger.Info(this, "Received package upload " + model.File.FileName);
				Core.Package package = BytePackage.Create(model.File.InputStream.ReadAllBytes(), model.File.FileName);
				Domain.Application application = this._repository.GetApplication(model.Application);
				this._repository.CreatePackage(application, package);
			}
			return this.RedirectToAction("List", new { application = model.Application });
		}

	}

}
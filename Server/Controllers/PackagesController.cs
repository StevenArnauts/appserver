using System.Web.Mvc;
using Core;
using Server.Persistence;
using Utilities;
using Utilities.MVC;
using Utilities.WebApi;

namespace Server {

	[HandleErrors]
	public class PackageController : BaseController {

		private readonly IApplicationRepository _repository;

		public PackageController(IApplicationRepository repository) {
			this._repository = repository;
		}

		[Trace]
		[ActionName("List")]
		[HttpGet]
		public ActionResult List(string application) {
			Persistence.Application app = this._repository.GetApplication(application);
			PackageListModel model = new PackageListModel { Packages = this._repository.GetPackages(app), Application = application };
			return this.View("List", model);
		}

		[Authorize]
		[Trace]
		[ActionName("Add")]
		[HttpGet]
		public ActionResult Add(string application) {
			PackageAddModel model = new PackageAddModel();
			model.Application = application;
			return this.View("Add", model);
		}

		[Authorize]
		[Trace]
		[ActionName("Add")]
		[HttpPost]
		public ActionResult AddSubmit(PackageAddModel model) {
			if(model.File != null && model.File.ContentLength > 0) {
				Logger.Info(this, "Received package upload " + model.File.FileName);
				Core.Package package = BytePackage.Create(model.File.InputStream.ReadAllBytes(), model.File.FileName);
				Persistence.Application fileSystemApplication = this._repository.GetApplication(model.Application);
				this._repository.CreatePackage(fileSystemApplication, package);
			}
			return this.RedirectToAction("List", new { application = model.Application });
		}

	}

}
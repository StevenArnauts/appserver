using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using AutoMapper;
using Server.Domain;
using Utilities.WebApi;

namespace Server {

	[TraceRequest]
	[HandleErrors]
	public class PackagesApiController : ApiController {

		private readonly IPackageRepository _packageRepository;

		public PackagesApiController(IPackageRepository packageRepository) {
			this._packageRepository = packageRepository;
		}

		//[HttpGet]
		//[Route("api/applications")]
		//public IEnumerable<Application> GetApplications() {
		//	return (this._packageRepository.GetApplications().Select(Mapper.Map<Application>));
		//}

		//[HttpGet]
		//[Route("api/applications/{name}")]
		//public Application GetApplication(string name) {
		//	return (Mapper.Map<Application>(this._packageRepository.GetApplication(name)));
		//}

		/// <summary>
		/// Returns all packages for the application, or all packages that are more recent but still compatible with the since version.
		/// </summary>
		/// <param name="name">The application name, e.g. Briljant</param>
		/// <param name="since">The minimum version of the package (must be greater)</param>
		/// <returns></returns>
		[HttpGet]
		[Route("api/applications/{name}/packages")]
		public IEnumerable<Package> GetPackages(string name, [ModelBinder(typeof(VersionModelBinder))] Version since = null) {
			Domain.Application application = this._packageRepository.GetApplication(name);
			return (this._packageRepository.GetPackages(application, since).Select(Mapper.Map<Package>));
		}

		[HttpGet]
		[Route("api/applications/{name}/packages/{version}")]
		public Package GetPackage(string name, [ModelBinder(typeof(VersionModelBinder))] Version version) {
			Domain.Application application = this._packageRepository.GetApplication(name);
			return (Mapper.Map<Package>(this._packageRepository.GetPackage(application, version)));
		}

		[HttpGet]
		[Route("api/applications/{name}/packages/{version}/bytes")]
		public HttpResponseMessage GetBytes(string name, [ModelBinder(typeof(VersionModelBinder))] Version version) {
			Domain.Application application = this._packageRepository.GetApplication(name);
			Domain.Package package = this._packageRepository.GetPackage(application, version);
			HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK) {
				Content = new StreamContent(this._packageRepository.GetPackage(package))
			};
			response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
			response.Content.Headers.ContentDisposition.FileName = Path.GetFileName(package.File);
			return response;
		}

	}

}
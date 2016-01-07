using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using AutoMapper;
using Core.Persistence;
using Utilities.WebApi;

namespace Server {

	[TraceRequest]
	[HandleErrors]
	public class PackagesApiController : ApiController {

		private readonly IApplicationRepository _packageRepository;

		public PackagesApiController(IApplicationRepository packageRepository) {
			this._packageRepository = packageRepository;
		}

		/// <summary>
		/// Returns all packages for the application, or all packages that are more recent but still compatible with the since version.
		/// </summary>
		/// <param name="name">The application name, e.g. Briljant</param>
		/// <param name="since">The minimum version of the package (must be greater)</param>
		/// <returns></returns>
		[HttpGet]
		[Route("api/applications/{name}/packages")]
		public IEnumerable<Package> GetPackages(string name, [ModelBinder(typeof(VersionModelBinder))] Version since = null) {
			FileSystemApplication fileSystemApplication = this._packageRepository.GetApplication(name);
			return (this._packageRepository.GetPackages(fileSystemApplication, since).Select(Mapper.Map<Package>));
		}

		[HttpGet]
		[Route("api/applications/{name}/packages/{version}")]
		public Package GetPackage(string name, [ModelBinder(typeof(VersionModelBinder))] Version version) {
			FileSystemApplication fileSystemApplication = this._packageRepository.GetApplication(name);
			return (Mapper.Map<Package>(this._packageRepository.GetPackage(fileSystemApplication, version)));
		}

		[HttpGet]
		[Route("api/applications/{name}/packages/{version}/bytes")]
		public HttpResponseMessage GetBytes(string name, [ModelBinder(typeof(VersionModelBinder))] Version version) {
			FileSystemApplication fileSystemApplication = this._packageRepository.GetApplication(name);
			FileSystemPackage package = this._packageRepository.GetPackage(fileSystemApplication, version);
			HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK) {
				Content = new StreamContent(this._packageRepository.GetPackage(package))
			};
			response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
			response.Content.Headers.ContentDisposition.FileName = Path.GetFileName(package.File);
			return response;
		}

	}

}
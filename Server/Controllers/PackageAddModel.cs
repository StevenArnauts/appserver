using System.Web;

namespace Server {

	public class PackageAddModel {

		public string Application { get; set; }
		public HttpPostedFileBase File { get; set; }

	}

}
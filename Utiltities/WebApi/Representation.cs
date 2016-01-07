using System.Collections.Generic;
using Newtonsoft.Json;

namespace Utilities.WebApi {

	public class Representation {

		public Representation() {
			this.Links = new Dictionary<string, Link>();
		}
		
		[JsonIgnore]
		//[JsonProperty("_links")]
		public Dictionary<string, Link> Links { get; set; }

	}

}
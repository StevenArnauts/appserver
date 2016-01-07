using System.Collections.Generic;
using System.Linq;

namespace Utilities.WebApi {

	public class LinkCollection : LinkedList<Link> {

		public bool ContainsLinkName(string name) {
			return (this.Any(l => l.Name == name));
		}

	}

}
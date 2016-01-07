using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core {

	class ProcessManager {

		public void Go() {
			Process p = new Process();
			p.Exited += P_Exited;
		}

		private void P_Exited(object sender, EventArgs e) {
			throw new NotImplementedException();
		}
	}

}

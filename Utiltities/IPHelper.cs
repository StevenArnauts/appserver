using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

namespace Utilities {

	public class IPHelper {

		public static int FindFreePort(int lowerBound, int upperBound) {
			int[] portsInUse = FindPortsInUse().ToArray();
			for(int port = lowerBound ; port <= upperBound; port++) {
				if (!portsInUse.Contains(port)) return (port);
			}
			return (-1);
		}

		public static IEnumerable<int> FindPortsInUse() {
			IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
			TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();
			return tcpConnInfoArray.Select(tcpi => (tcpi.LocalEndPoint.Port));
		}

	}

}
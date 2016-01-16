using System;
using System.Threading;

namespace Core.Contract {

	public interface IBootstrapper : IDisposable {

		void Initialize(ServerContext context);
		void Run(CancellationToken token);

	}

}
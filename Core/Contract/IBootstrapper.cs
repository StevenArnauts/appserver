using System;
using System.Threading;

namespace Core.Contract {

	public interface IBootstrapper : IDisposable {

		void Initialize(Context context);
		void Run(CancellationToken token);

	}

}
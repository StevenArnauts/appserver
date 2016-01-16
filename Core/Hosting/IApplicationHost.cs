using Core.Contract;

namespace Core {

	public interface IApplicationHost {

		void Initialize(ServerContext context, Type bootstrapper);
		void Start();
		void Stop();
		void Destroy();

	}

}
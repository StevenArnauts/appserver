using Core.Contract;

namespace Core {

	public interface IApplicationHost {

		void Initialize(ServerContext context, Type bootstrapper, string[] args);
		void Start();
		void Stop();
		void Destroy();

	}

}
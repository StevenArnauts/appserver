namespace Core.Contract {

	public interface IHost {

		void Initialize(Type bootstrapper, ServerContext context, string[] args);
		void Start();
		void Stop();

	}

}
namespace Core.Contract {

	public interface IHost {

		void Initialize(Type bootstrapper, ServerContext context);
		void Start();
		void Stop();

	}

}
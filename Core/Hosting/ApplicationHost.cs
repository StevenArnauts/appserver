using Core.Contract;

namespace Core {

	internal abstract class ApplicationHost {

		internal abstract void Initialize(Context context, Type bootstrapper);
		internal abstract void Start();
		internal abstract void Stop();
		internal abstract void Destroy();

	}

}
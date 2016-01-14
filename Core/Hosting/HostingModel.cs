namespace Core {

	public abstract class HostingModel {

		internal abstract ApplicationHost Create(string binFolder, string assembly);

	}

}
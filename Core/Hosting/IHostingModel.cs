namespace Core {

	public interface IHostingModel {

		IApplicationHost Create(string binFolder, string assembly);

	}

}
using System;

namespace Core.Contract {

	[Serializable]
	public class Message {

		public string Id { get; set; }
		public string Content { get; set; }

	}

}
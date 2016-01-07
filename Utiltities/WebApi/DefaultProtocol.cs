using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Utilities.WebApi {

	/// <summary>
	/// The default REST protocol for WKB.
	/// </summary>
	public static class DefaultProtocol {

		static DefaultProtocol() {
			Encoding = Encoding.UTF8;
			Serializer = new DefaultSerializer();
		}

		public static Encoding Encoding { get; private set; }
		public static DefaultSerializer Serializer { get; private set; }

		public class DefaultSerializer {

			private readonly JsonSerializer _serializer;

			public DefaultSerializer() {
				this._serializer = new JsonSerializer();
				this._serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
				this._serializer.ContractResolver = new CamelCasePropertyNamesContractResolver();
			}

			public T Deserialize<T>(string content) {
				using (StringReader reader = new StringReader(content)) {
					return (this.Deserialize<T>(reader));
				}
			}

			public T Deserialize<T>(Stream stream) {
				StreamReader reader = new StreamReader(stream);
				return (this.Deserialize<T>(reader));
			}

			public T Deserialize<T>(TextReader reader) {
				return ( (T)this._serializer.Deserialize(reader, typeof(T)) );
			}

			public void Populate(Stream stream, object target) {
				StreamReader reader = new StreamReader(stream);
				this.Populate(reader, target);
			}

			public void Populate(TextReader reader, object target) {
				this._serializer.Populate(reader, target);
			}

			public string Serialize(object content) {
				StringBuilder builder = new StringBuilder();
				using(StringWriter writer = new StringWriter(builder)) {
					this._serializer.Serialize(writer, content);
				}
				return (builder.ToString());
			}

			public void Serialize(object content, Stream stream) {
				using(StreamWriter writer = new StreamWriter(stream)) {
					this._serializer.Serialize(writer, content);
				}
			}

		}

	}

}
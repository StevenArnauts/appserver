using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Utilities.WebApi {

	public abstract class BaseJsonMediaTypeFormatter : MediaTypeFormatter {

		private readonly JsonSerializer _serializer;

		protected BaseJsonMediaTypeFormatter() {
			this._serializer = JsonSerializerFactory.Create();
		}

		public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext) {
			return (Task.Factory.StartNew(() => this.WriteToStream(value, writeStream)));
		}

		public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger) {
			return (Task<object>.Factory.StartNew(() => this.ReadFromStream(type, readStream, content, formatterLogger)));
		}

		private object ReadFromStream(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger) {
			return (this.ReadObject(readStream, type));
		}

		private void WriteToStream(object value, Stream writeStream) {
			IEnumerable<Representation> list = value as IEnumerable<Representation>;
			if (list != null) {
				List<Representation> newList = list.ToList();
				newList.ForEach(this.Prepare);
				this.WriteObject(newList, writeStream);
			} else {
				Representation representation = value as Representation;
				this.Prepare(representation);
				this.WriteObject(value, writeStream);
			}
		}

		protected virtual void Prepare(Representation representation) {}

		private object ReadObject(Stream stream, Type type) {
			if (type.IsInterface) return null;
			StreamReader reader = new StreamReader(stream);
			return (this._serializer.Deserialize(reader, type));
		}

		private void WriteObject(object value, Stream stream) {
			StreamWriter writer = new StreamWriter(stream);
			//to prevent code injection, prefix all json with ")]}',\n" to make it non-executable. 
			//See http://haacked.com/archive/2008/11/20/anatomy-of-a-subtle-json-vulnerability.aspx/
			writer.Write(")]}',\n");
			this._serializer.Serialize(writer, value);
			writer.Flush();
		}

	}

}
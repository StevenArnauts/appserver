using System;
using System.Net.Http.Headers;

namespace Utilities.WebApi {

	//public class HalPlusJsonMediaTypeFormatter : BaseJsonMediaTypeFormatter {

	//	public HalPlusJsonMediaTypeFormatter() {
	//		this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/hal+json"));
	//	}

	//	protected override void Prepare(Representation representation) {
	//		base.Prepare(representation);
	//		representation.IncludeHypermedia = true;
	//		if (representation.HypermediaFactory != null) representation.HypermediaFactory.Invoke(representation);
	//	}

	//	public override bool CanReadType(Type type) {
	//		bool canRead = type.IsSubclassOf(typeof (Representation));
	//		Logger.Debug(this, "can read " + type.FullName + " : " + canRead);
	//		return (canRead);
	//	}

	//	public override bool CanWriteType(Type type) {
	//		bool isListOfRepresentationOrSubtype = type.IsListOf(typeof (Representation));
	//		bool isTypeOrSubTypeOfRepresentation = type.IsSubclassOf(typeof (Representation));
	//		bool canWrite = isListOfRepresentationOrSubtype || isTypeOrSubTypeOfRepresentation;
	//		Logger.Debug(this, "can write " + type.FullName + " : " + canWrite);
	//		return (canWrite);
	//	}

	//}

}
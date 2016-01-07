using Microsoft.Web.XmlTransform;
using Utilities;

namespace Alure {

	/// <summary>
	/// Transforms an xml file using an xdt transformation.
	/// Read about xdt here: https://msdn.microsoft.com/en-us/library/dd465326.aspx. 
	/// </summary>
	internal class FileTransformer {

		private readonly string _sourceFile;
		private readonly string _targetFile;
		private readonly string _transformationfile;

		public FileTransformer(string sourceFile, string targetFile, string transformationfile) {
			this._targetFile = targetFile;
			this._transformationfile = transformationfile;
			this._sourceFile = sourceFile;
		}

		public void Run() {
			Logger.Info(this, "Transforming " + this._sourceFile + " using " + this._transformationfile + " to " + this._targetFile + "...");
			using (XmlTransformableDocument document = new XmlTransformableDocument()) {
				document.PreserveWhitespace = true;
				document.Load(this._sourceFile);
				using (XmlTransformation transform = new XmlTransformation(this._transformationfile)) {
					transform.Apply(document);
					Logger.Debug(this, "Result");
					Logger.Debug(this, document.OuterXml);
					document.Save(this._targetFile);
					Logger.Info(this, "Saved to " + this._targetFile);
				}
			}
		}

	}

}
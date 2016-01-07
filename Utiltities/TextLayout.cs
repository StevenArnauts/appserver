using System;
using System.Globalization;
using System.IO;
using System.Threading;
using log4net.Core;
using log4net.Layout;

namespace Utilities {

	public class TextLayout : LayoutSkeleton {

		public override void Format(TextWriter writer, LoggingEvent loggingEvent) {
			// check arguments
			if (loggingEvent == null) return;
			if (loggingEvent.MessageObject == null && loggingEvent.RenderedMessage == null) return;

			// get logger id
			string loggerId = loggingEvent.GetLoggingEventProperty("__objectId");

			// prepare stuff
			string message = loggingEvent.MessageObject == null ? loggingEvent.RenderedMessage : loggingEvent.MessageObject.ToString();
			string info = loggingEvent.GetLoggingEventProperty("__extendedInfo");
			if (info != null) {
				message = message + " " + info;
			}
			string[] lines = message.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			string header = string.Format("{0} [{1}] ({3}|{4}|{5}) {2} : ", loggingEvent.TimeStamp.ToString("dd-MM-yyyy hh:mm:ss,fff"), loggingEvent.Level.DisplayName.PadLeft(5, ' '), this.LoggerName(loggingEvent.LoggerName), Thread.CurrentThread.GetHashCode().ToString(CultureInfo.InvariantCulture).PadLeft(2, ' '), loggerId.PadLeft(2), Thread.CurrentPrincipal != null ? Thread.CurrentPrincipal.Identity.Name : "unknown");
			const string FILLER = "\t";

			for (int i = 0; i < lines.Length; i++) {
				if (i == 0) {
					writer.Write(header);
				} else {
					writer.Write(FILLER);
				}
				writer.WriteLine(lines[i]);
			}
		}

		public override void ActivateOptions() {}

		private string LoggerName(string logger) {
			if (logger.Contains("<") && logger.Contains(">")) {
				int genStart = logger.IndexOf('<');
				return (this.RemoveNamespace(logger.Substring(0, genStart)) + "<" + this.RemoveNamespace(logger.Substring(genStart + 1, logger.IndexOf(">", StringComparison.Ordinal) - genStart - 1)) + ">");
			} else if (logger.Contains("[[") && logger.Contains("]]")) {
				int genStart = logger.IndexOf("[[", StringComparison.Ordinal);
				return (this.RemoveNamespace(logger.Substring(0, genStart)) + "<" + this.RemoveNamespace(logger.Substring(genStart + 1, logger.IndexOf("]]", StringComparison.Ordinal) - genStart - 1)) + ">");
			} else {
				return (this.RemoveNamespace(logger));
			}
		}

		/// <summary>
		/// Strips the namespace from the logger/source.
		/// </summary>
		/// <param name="logger"></param>
		/// <returns></returns>
		private string RemoveNamespace(string logger) {
			if (logger == null) return (null);
			int pos = logger.LastIndexOf(".", StringComparison.Ordinal);
			if (pos >= 0 && pos < (logger.Length - 1)) {
				return (logger.Substring(pos + 1, logger.Length - 1 - pos));
			}
			return (logger);
		}

	}

	public static class LoggingEventExtensions {

		public static string GetLoggingEventProperty(this LoggingEvent loggingEvent, string key) {
			string value = string.Empty;
			if (loggingEvent.Properties != null && loggingEvent.Properties.Contains(key) && loggingEvent.Properties[key] != null) {
				value = loggingEvent.Properties[key].ToString();
			}
			return value;
		}

	}

}
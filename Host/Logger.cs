using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Host {

	internal class Logger {

		internal static void Log(string message) {
			object source = GetCaller();
			Write(FormatSender(source) ,message);
		}

		internal static void Log(string message, Exception error) {
			object source = GetCaller();
			string line = message + Environment.NewLine + FormatException(error);
			Write(FormatSender(source), line);
		}

		private static void Write(string source, string message) {
			Console.WriteLine("{0} | {1} : {2}", DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss,fff"), LoggerName(source), message);
		}

		public static string FormatSender(object sender) {
			if(sender == null) {
				return "Application";
			}

			if(sender is string) {
				return (string)sender;
			}

			if(sender is Type) {
				return ((Type)sender).FullName;
			}

			Type type = sender.GetType();
			MethodInfo method = type.GetMethod("ToString", BindingFlags.DeclaredOnly);
			if(method == null) {
				if(type.IsGenericType) {
					int typeEnd = type.FullName.IndexOf('`');
					int genericTypeStart = type.FullName.IndexOf("[[", StringComparison.Ordinal) + 2;
					int genericTypeEnd = type.FullName.IndexOf(',');
					string typeName = type.FullName.Substring(0, typeEnd);
					string genericName = type.FullName.Substring(genericTypeStart, genericTypeEnd - genericTypeStart);
					string result = typeName + "<" + genericName + ">";
					return (result);
				} else {
					return sender.GetType().FullName;
				}
			} else {
				return ((string)method.Invoke(sender, null));
			}
		}

		private static string FormatException(Exception ex) {
			Exception e = ex;
			StringBuilder bldr = new StringBuilder();
			while(e != null) {
				bldr.AppendLine(string.Format("Type: {0}", e.GetType()));
				bldr.AppendLine(string.Format("Message: {0}", e.Message));
				foreach(object key in e.Data.Keys) {
					bldr.AppendLine("Data[" + key + "] = " + e.Data[key]);
				}
				bldr.AppendLine("Stack Trace: ");
				bldr.Append(e.StackTrace);
				e = e.InnerException;
				if(e != null) {
					bldr.AppendLine();
					bldr.AppendLine("Caused by: ");
				}
			}
			return (bldr.ToString());
		}

		private static Type GetCaller() {
			StackTrace t = new StackTrace();
			for(int i = 0; i < t.FrameCount; i++) {
				MethodBase m = t.GetFrame(i).GetMethod();
				if(m.ReflectedType != typeof(Logger)) {
					return m.ReflectedType;
				}
			}
			return null;
		}

		private static string LoggerName(string logger) {
			if(logger.Contains("<") && logger.Contains(">")) {
				int genStart = logger.IndexOf('<');
				return (RemoveNamespace(logger.Substring(0, genStart)) + "<" + RemoveNamespace(logger.Substring(genStart + 1, logger.IndexOf(">", StringComparison.Ordinal) - genStart - 1)) + ">");
			} else if(logger.Contains("[[") && logger.Contains("]]")) {
				int genStart = logger.IndexOf("[[", StringComparison.Ordinal);
				return (RemoveNamespace(logger.Substring(0, genStart)) + "<" + RemoveNamespace(logger.Substring(genStart + 1, logger.IndexOf("]]", StringComparison.Ordinal) - genStart - 1)) + ">");
			} else {
				return (RemoveNamespace(logger));
			}
		}

		private static string RemoveNamespace(string logger) {
			if(logger == null) return (null);
			int pos = logger.LastIndexOf(".", StringComparison.Ordinal);
			if(pos >= 0 && pos < (logger.Length - 1)) {
				return (logger.Substring(pos + 1, logger.Length - 1 - pos));
			}
			return (logger);
		}


	}

}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;
using log4net;
using log4net.Config;
using log4net.Core;
using log4net.Util;

namespace Utilities {

	public delegate string LoggingStatement();

	/// <summary>
	/// Wrapper class to log directly to log4net.
	/// </summary>
	public static class Logger {

		/// <summary>
		/// doubly indexed collection: first by the object's type hashcode, secondly by it's own hashcode
		/// </summary>
		private static readonly Dictionary<int, Dictionary<int, int>> mapping = new Dictionary<int, Dictionary<int, int>>();

		/// <summary>
		/// The location of the log file
		/// </summary>
		// private static string Trace = @"logger.log";
		/// <summary>
		/// Writes the specified text to a file at the provided location.
		/// </summary>
		/// <param name="file">The path to the file.</param>
		/// <param name="text">The text to be written.</param>
		public static void WriteToFile(string file, string text) {
			FileStream stream;
			if(File.Exists(file)) {
				stream = File.OpenWrite(file);
				stream.Position = stream.Length;
			} else {
				stream = File.Create(file);
			}

			byte[] message = Encoding.Default.GetBytes((DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") + text + Environment.NewLine).ToCharArray());
			stream.Write(message, 0, message.Length);
			stream.Close();
		}

		/// <summary>
		/// Initializes the specified file to be used for logging purposes.
		/// </summary>
		/// <param name="file">The file.</param>
		public static void Initialize(string file) {
			string path;
			Console.WriteLine("Used path " + file);
			if(HttpContext.Current != null) {
				path = HttpContext.Current.Server.MapPath(file);
			} else {
				if(Path.IsPathRooted(file)) path = file;
				else {
					Assembly a = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
					string location = a.Location;
					string root = new FileInfo(location).Directory.FullName;
					path = Path.Combine(root, file);
				}
			}

			XmlConfigurator.Configure(new FileInfo(path));
		}

		/// <summary>
		/// Returns a unique integer, as small as possible, for the object.
		/// <remarks>This is of course VERY SLOW AND CAUSES MEMORY LEAKS, 
		/// be carefull when using it.</remarks>
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public static int Identify(object o) {
			if(o == null) return -1;
			lock (mapping) {
				// make the type map if needed (first level index)
				int typeCode = o.GetType().GetHashCode();
				if(!mapping.ContainsKey(typeCode)) {
					Dictionary<int, int> map = new Dictionary<int, int>();
					mapping.Add(typeCode, map);
				}

				// now get the object
				int objectCode = o.GetHashCode();
				if(mapping[typeCode].ContainsKey(objectCode)) {
					return (mapping[typeCode][objectCode]);
				} else {
					int nr = mapping[typeCode].Count + 1;
					mapping[typeCode].Add(objectCode, nr);
					return (nr);
				}
			}
		}

		private static object GetSenderId(object sender) {
			object id;
			if(sender == null) {
				id = "n/a";
			} else if(sender is Type) {
				id = "T";
			} else if(sender is string) {
				id = "S";
			} else {
				id = Identify(sender);
			}
			return (id);
		}

		private static bool IsLogLevelEnabledFor(Level level, ILog log) {
			if(level == Level.Debug) return (log.IsDebugEnabled);
			if(level == Level.Info) return (log.IsInfoEnabled);
			if(level == Level.Warn) return (log.IsWarnEnabled);
			if(level == Level.Error) return (log.IsErrorEnabled);
			return (false);
		}

		private static void SendMessage(Level level, object sender, string message, object extendedInfo = null) {
			string logger = FormatSender(sender);
			ILog log = LogManager.GetLogger(logger);

			if(IsLogLevelEnabledFor(level, log)) {
				LoggingEventData data = new LoggingEventData();
				data.TimeStamp = DateTime.Now;
				data.Level = level;
				data.LoggerName = logger;
				data.Message = message;
				data.Properties = new PropertiesDictionary();
				data.Properties["__objectId"] = GetSenderId(sender);
				data.Properties["__extendedInfo"] = extendedInfo;
				LoggingEvent evt = new LoggingEvent(data);
				log.Logger.Log(evt);
			}
		}

		/// <summary>
		/// Logs the specified message with level Debug.
		/// </summary>
		/// <param name="sender">The sender of the message.</param>
		/// <param name="message">The message.</param>
		public static void Debug(object sender, string message) {
			SendMessage(Level.Debug, sender, message);
		}

		/// <summary>
		/// Logs the specified message with level Info.
		/// </summary>
		/// <param name="sender">The sender of the message.</param>
		/// <param name="message">The message.</param>
		/// /// <param name="extendedInfo">Any extra information</param>
		public static void Info(object sender, string message, object extendedInfo) {
			SendMessage(Level.Info, sender, message, extendedInfo);
		}

		/// <summary>
		/// Generates a debug line with the provided message.
		/// </summary>
		/// <param name="message">The message.</param>
		public static void Debug(string message) {
			Type caller = GetCaller();
			Debug(caller, message);
		}

		/// <summary>
		/// Overload that takes a statement as argument instead of a string. Use this to wrap
		/// intensive string manipulation in a (anonymous) delegate that will be called only
		/// when the loglevel is Debug (-> save some CPU and memory in PRD)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="statement"></param>
		/// <example>
		/// Logger.Debug(this.Pipe.Name, () => "Error passing message to filter " + fh.Filter.Name + ": " + ex.Message + " (" + ex.GetType().FullName + ")");
		/// </example>
		public static void Debug(object sender, LoggingStatement statement) {
			ILog log = LogManager.GetLogger(FormatSender(sender));
			if(log.IsDebugEnabled) {
				log.Debug(statement.Invoke());
			}
		}

		/// <summary>
		/// Generates an info line with the provided message.
		/// </summary>
		/// <param name="message">The message.</param>
		public static void Info(string message) {
			Info(GetCaller(), message);
		}

		/// <summary>
		/// Logs the specified message with level Informational.
		/// </summary>
		/// <param name="sender">The sender of the message.</param>
		/// <param name="message">The message.</param>
		public static void Info(object sender, string message) {
			SendMessage(Level.Info, sender, message);
		}

		/// <summary>
		/// Logs the specified message with level Informational. This overload is specifically
		/// made for logging statements that require costly formatting of the logged message.
		/// </summary>
		/// <param name="sender">The sender of the message.</param>
		/// <param name="statement">A delegate that returns the message to log.</param>
		/// <example>
		/// Logger.Info(this, () => ("Starting..."));
		/// Logger.Info(this, delegate { return("Starting..."); });
		/// </example>
		public static void Info(object sender, LoggingStatement statement) {
			ILog log = LogManager.GetLogger(FormatSender(sender));
			if(log.IsInfoEnabled) {
				log.Info(statement.Invoke());
			}
		}

		/// <summary>
		/// Generates an info line with the provided message.
		/// </summary>
		/// <param name="message">The message.</param>
		public static void Warn(string message) {
			Warn(GetCaller(), message);
		}

		/// <summary>
		/// Logs the specified message with level Warning.
		/// </summary>
		/// <param name="sender">The sender of the message.</param>
		/// <param name="message">The message.</param>
		public static void Warn(object sender, string message) {
			SendMessage(Level.Warn, sender, message);
		}

		/// <summary>
		/// Logs the specified message with level Warning.
		/// </summary>
		/// <param name="sender">The sender of the message.</param>
		/// <param name="ex">The exception to log as a warning.</param>
		/// <param name="message">The message.</param>
		public static void Warn(object sender, Exception ex, string message) {
			SendMessage(Level.Warn, sender, message + Environment.NewLine + FormatException(ex));
		}

		/// <summary>
		/// Logs the specified message with level Warning.
		/// </summary>
		/// <param name="sender">The sender of the message.</param>
		/// <param name="statement">A delegate to a statement returning the message to log.</param>
		public static void Warn(object sender, LoggingStatement statement) {
			ILog log = LogManager.GetLogger(FormatSender(sender));
			if(log.IsWarnEnabled) {
				log.Warn(statement.Invoke());
			}
		}

		/// <summary>
		/// Logs the specified message with level Exception.
		/// </summary>
		/// <param name="sender">The sender of the message.</param>
		/// <param name="ex">The exception.</param>
		public static void Exception(object sender, Exception ex) {
			ILog log = LogManager.GetLogger(FormatSender(sender));
			if(log.IsErrorEnabled) {
				log.Error(FormatException(ex));
			}
		}

		/// <summary>
		/// Logs the specified message with level Exception.
		/// </summary>
		/// <param name="sender">The sender of the message.</param>
		/// <param name="ex">The exception.</param>
		/// <param name="message">The message.</param>
		public static void Exception(object sender, Exception ex, string message) {
			ILog log = LogManager.GetLogger(FormatSender(sender));
			if(log.IsErrorEnabled) {
				log.Error(message + Environment.NewLine + FormatException(ex));
			}
		}

		/// <summary>
		/// Logs the specified message with level Error.
		/// </summary>
		/// <param name="ex">The exception.</param>
		public static void Error(Exception ex) {
			ILog log = LogManager.GetLogger(FormatSender(GetCaller()));
			if(log.IsErrorEnabled) {
				log.Error(FormatException(ex));
			}
		}

		/// <summary>
		/// Logs the specified message with level Error.
		/// </summary>
		public static void Error(string message) {
			ILog log = LogManager.GetLogger(FormatSender(GetCaller()));
			if(log.IsErrorEnabled) {
				log.Error(message);
			}
		}

		/// <summary>
		/// Logs the specified message with level Error.
		/// </summary>
		/// <param name="sender">The sender of the message.</param>
		/// <param name="ex">The exception.</param>
		public static void Error(object sender, Exception ex) {
			SendMessage(Level.Error, sender, FormatException(ex));
		}

		/// <summary>
		/// Logs the specified message with level Error.
		/// </summary>
		/// <param name="ex">The exception.</param>
		/// <param name="message">The message.</param>
		public static void Error(Exception ex, string message) {
			ILog log = LogManager.GetLogger(FormatSender(GetCaller()));
			if(log.IsErrorEnabled) {
				log.Error(message + Environment.NewLine + FormatException(ex));
			}
		}

		/// <summary>
		/// Logs the specified message with level Error.
		/// </summary>
		/// <param name="sender">The sender of the message.</param>
		/// <param name="ex">The exception.</param>
		/// <param name="message">The message.</param>
		public static void Error(object sender, Exception ex, string message) {
			SendMessage(Level.Error, sender, message + Environment.NewLine + FormatException(ex));
		}

		/// <summary>
		/// Deduces an event source name from an object: if the object is actually a type,
		/// the type name is returned, otherwise the name of the object's type.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <returns>The type name</returns>
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

		/// <summary>
		/// Formats any object by calling <see cref="Object.ToString()"/>. Special cases are
		///		null: formatted as "(null)"
		///		<see cref="string"/>: put between quotes
		///		<see cref="IEnumerable"/>: formatted as "{item,item}"
		///		<see cref="NameValueCollection"/> formatted as "{key=value, key=value}"
		///	Protection against stack overflows is provided: any object is formatted only once, if it's formatted
		/// a second time, "-" is returned.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[DebuggerStepThrough]
		public static string Format(object value) {
			return (Format(value, ", ", "="));
		}

		/// <summary>
		/// Formats any object.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <param name="itemSeparator">The string to put between items in a collection</param>
		/// <param name="nameValueSeparator">The string to put between the name and the value of the items of <see cref="NameValueCollection"/>.</param>
		[DebuggerStepThrough]
		public static string Format(object value, string itemSeparator, string nameValueSeparator) {
			List<object> formattedObjects = new List<object>();
			return (FormatValue(value, formattedObjects, itemSeparator, nameValueSeparator));
		}

		/// <summary>
		/// Format an exception to a nicely readable string representation.
		/// </summary>
		/// <param name="ex">The exception data.</param>
		/// <returns>A formatted string of the exception.</returns>
		public static string FormatException(Exception ex) {
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

		public static Exception GetRootCause(this Exception source) {
			Exception e = source;
			while (e.InnerException != null) {
				e = e.InnerException;
			}
			return (e);
		}

		/// <summary>
		/// Returns the calling method, i.e. the first method that does not belong to the
		/// Logger class.
		/// </summary>
		/// <returns>The caller of the log functionality</returns>
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

		/// <summary>
		/// Formats an object as a value. Mostly this means "ToString-ing" it, except for
		/// strings which are put between quotes.
		/// </summary>
		/// <param name="value">The object to format</param>
		/// <param name="formattedObjects">A list containing references to objects already formatted, to avoid stack overflows.</param>
		/// /// <returns></returns>
		/// <param name="itemSeparator">The string to put between items in a collection</param>
		/// <param name="nameValueSeparator">The string to put between the name and the value of the items of <see cref="NameValueCollection"/>.</param>
		[DebuggerStepThrough]
		private static string FormatValue(object value, ICollection<object> formattedObjects, string itemSeparator, string nameValueSeparator) {
			if(formattedObjects.Contains(value)) {
				Logger.Debug(value + "(" + value.GetHashCode() + " already formatted");
				return ("-");
			} else {
				string result;
				if(value == null) {
					result = "(null)";
				} else {
					if(!value.GetType().IsValueType) formattedObjects.Add(value);

					if(value is string) {
						result = "\"" + value + "\"";
					} else {
						if(value is NameValueCollection) {
							NameValueCollection collection = value as NameValueCollection;
							result = "{";
							for(int i = 0; i < collection.Count; i++) {
								result += collection.Keys[i] + nameValueSeparator + FormatValue(collection[i], formattedObjects, itemSeparator, nameValueSeparator);
								if(i < collection.Count - 1) result += itemSeparator;
							}
							result += "}";
						} else {
							if(value is IEnumerable && !IsToStringOverridden(value)) {
								result = "{";
								foreach(object val in (IEnumerable)value) {
									result += FormatValue(val, formattedObjects, itemSeparator, nameValueSeparator) + itemSeparator;
								}

								if(result.EndsWith(itemSeparator)) result = result.Substring(0, result.Length - itemSeparator.Length);
								result += "}";
							} else {
								result = value.ToString();
							}
						}
					}
				}
				return (result);
			}
		}

		private static bool IsToStringOverridden(Object value) {
			Type type = value.GetType();

			Type[] types = new Type[0];
			MethodInfo method = type.GetMethod("ToString", types);

			return method.DeclaringType == type && method.GetBaseDefinition().DeclaringType == typeof(Object);
		}

	}
}

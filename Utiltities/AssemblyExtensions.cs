using System;
using System.Reflection;

namespace Utilities {

	public static class AssemblyExtensions {

		/// <summary>
		/// Returns the file name of the assembly
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static string Path(this Assembly source) {
			return (System.IO.Path.GetFileName(new Uri(source.CodeBase).LocalPath));
		}

		public static string Name(this Assembly source) {
			return (source.GetName().Name);
		}

		public static string Version(this Assembly source) {
			return ( source.GetName().Version.ToString(4) );
		}

	}

}
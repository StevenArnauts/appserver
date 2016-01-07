//using System.ComponentModel;

//namespace Utilities.WebApi {

//	[TypeConverter(typeof(VersionConverter))]
//	public class Version {

//		public int Major { get; set; }
//		public int Minor { get; set; }
//		public int Revision { get; set; }
//		public int Build { get; set; }

//		public static bool TryParse(string s, out Version result) {
//			result = null;
//			var parts = s.Split('.');
//			if(parts.Length != 4) {
//				return false;
//			}
//			int major, minor, revision, build;
//			if(int.TryParse(parts[0], out major) && int.TryParse(parts[1], out minor) && int.TryParse(parts[2], out revision) && int.TryParse(parts[3], out build)) {
//				result = new Version { Major = major, Minor = minor, Revision = revision, Build = build };
//				return true;
//			}
//			return false;
//		}

//		public static bool operator > (Version left, Version right) {
//			System.Version v1 = new System.Version();
//			System.Version v2;
//			return (v1 > v2);
//			return (left != null && right != null && (left.Major > right.Major || left.Major ));
//		}

//		public static bool operator < (Version left, Version right) {

//		}

//	}

//}
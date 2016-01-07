//using System;
//using System.ComponentModel;
//using System.Globalization;

//namespace Utilities.WebApi {

//	public class VersionConverter : TypeConverter {

//		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
//			if(sourceType == typeof(string)) {
//				return true;
//			}
//			return base.CanConvertFrom(context, sourceType);
//		}

//		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
//			if(value is string) {
//				Version version;
//				if(Version.TryParse((string)value, out version)) {
//					return version;
//				}
//			}
//			return base.ConvertFrom(context, culture, value);
//		}
//	}

//}
using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Utilities.WebApi {

	public class Link {

		public Link() {}

		public Link(string href, bool templated = false, string name = null) {
			this.Href = href;
			this.Name = name;
			this.Templated = templated;
			if (href != null) this.Templated = Regex.Match(href, @"{\w+}", RegexOptions.Compiled).Success;
		}

		public string Href { get; set; }
		public string Name { get; set; }
		public bool Templated { get; set; }

		public bool ContainsLinkName(string name) {
			return (false);
		}

		/// <summary>
		/// If this link is templated, you can use this method to make a non templated copy
		/// </summary>
		/// <param name="parameters">The parameters, i.e 'new {id = "1"}'</param>
		/// <returns>A non templated link</returns>
		public Link CreateLink(object parameters) {
			return new Link(this.Href, false, this.CreateUri(parameters).ToString());
		}

		public Uri CreateUri(object parameters, UriKind kind = UriKind.Relative) {
			string href = this.Href;
			foreach (PropertyInfo substitution in parameters.GetType().GetProperties()) {
				string name = substitution.Name;
				object value = substitution.GetValue(parameters, null);
				string substituionValue = value == null ? null : Uri.EscapeDataString(value.ToString());
				href = href.Replace(string.Format("{{{0}}}", name), substituionValue);
			}
			return new Uri(href, kind);
		}

	}

}
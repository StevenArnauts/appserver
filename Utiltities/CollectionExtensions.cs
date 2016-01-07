using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Utilities {

	public class QueryException : ApplicationException {

		protected QueryException() { }
		protected QueryException(string message) : base(message) { }

	}

	/// <summary>
	/// Is thrown when no single item in the collection satisfied the condition
	/// </summary>
	public class ObjectNotFoundException : QueryException {

		public ObjectNotFoundException() { }
		public ObjectNotFoundException(string message) : base(message) { }

	}

	/// <summary>
	/// Is thrown when more than one item in the collection statisfied the condition
	/// </summary>
	public class ObjectNotUniqueException : QueryException {

		public ObjectNotUniqueException() { }
		public ObjectNotUniqueException(string message) : base(message) { }

	}

	public static class CollectionExtenders {

		public static T Get<T>(this IQueryable<T> source, Expression<Func<T, bool>> condition) {
			List<T> candidates = source.Where(condition).ToList();
			if (!candidates.Any()) throw new ObjectNotFoundException("No instance of " + typeof(T).Name + " satisfied the criteria");
			if (candidates.Count() > 1) throw new ObjectNotUniqueException("There were " + candidates.Count() + " instances of " + typeof(T).Name + " that satisfied the criteria");
			return (candidates[0]);
		}

		public static T Get<T>(this IEnumerable<T> source, Predicate<T> condition) {
			List<T> candidates = source.Where(i => condition(i)).ToList();
			if (!candidates.Any()) throw new ObjectNotFoundException("No instance of " + typeof(T).Name + " satisfied the criteria");
			if (candidates.Count() > 1) throw new ObjectNotUniqueException("There were " + candidates.Count() + " instances of " + typeof(T).Name + " that satisfied the criteria");
			return (candidates[0]);
		}

		/// <summary>
		/// Tries to get a unique item from the list.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="condition"></param>
		/// <param name="result"></param>
		/// <returns>True if the item was found and was unique, false otherwise</returns>
		public static bool TryGet<T>(this IEnumerable<T> source, Predicate<T> condition, out T result) {
			try {
				result = Get(source, condition);
				return (true);
			} catch (QueryException) {
				result = default(T);
				return (false);
			}
		}

		public static string Print<T>(this IEnumerable<T> source, string separator) {
			StringBuilder bldr = new StringBuilder();
			IList<T> items = new List<T>(source);
			for (int i = 0; i < items.Count; i++) {
				bldr.Append(items[i]);
				if (i < items.Count - 1) bldr.Append(separator);
			}
			return (bldr.ToString());
		}


		public static void ForEach<T>(this ICollection<T> collection, Action<T> action) {
			foreach(T item in collection) action(item);
		}

		public static string Print<TKey, TItem>(this Dictionary<TKey, TItem> source, string valueSeparator = "=", string pairSeparator = ", ") {
			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < source.Count; i++) {
				KeyValuePair<TKey, TItem> valuePair = source.ElementAt(i);
				builder.Append(valuePair.Key);
				builder.Append(valueSeparator);
				builder.Append(valuePair.Value);
				if (i < (source.Count - 1)) builder.Append(pairSeparator);
			}
			return (builder.ToString());
		}

		public static string Print<TKey, TItem>(this Dictionary<TKey, TItem> source, Func<TItem, string> printer, string valueSeparator = "=", string pairSeparator = ", ") {
			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < source.Count; i++) {
				KeyValuePair<TKey, TItem> valuePair = source.ElementAt(i);
				builder.Append(valuePair.Key);
				builder.Append(valueSeparator);
				builder.Append(printer(valuePair.Value));
				if (i < (source.Count - 1)) builder.Append(pairSeparator);
			}
			return (builder.ToString());
		}

	}

}
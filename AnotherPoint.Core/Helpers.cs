using System.Collections.Generic;

namespace AnotherPoint.Core
{
	public static class Helpers
	{
		private static readonly IDictionary<string, string> correctCollectionTypeNaming = new Dictionary<string, string>
		{
			{"IEnumerable`1", "System.Collections.Generic.IEnumerable" }
		};

		private static readonly IDictionary<string, string> implementTypeNaming = new Dictionary<string, string>
		{
			{ "System.Collections.Generic.IEnumerable", "System.Collections.Generic.List" },
		};

		public static string GetCorrectCollectionTypeNaming(string key)
		{
			string value;

			return correctCollectionTypeNaming.TryGetValue(key, out value) ? value : key;
		}

		public static string GetImplementTypeNaming(string key)
		{
			string value;

			int v = key.IndexOf("<");

			if (v >= 0)
			{
				key = key.Remove(v);
			}

			return implementTypeNaming.TryGetValue(key, out value) ? value : key;
		}
	}
}
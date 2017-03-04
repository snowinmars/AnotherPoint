using System.Collections.Generic;
using System.Reflection;

namespace AnotherPoint.Common
{
	public static class Constant
	{
		public const string Abstract = " abstract ";
		public const BindingFlags AllInstance = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
		public const BindingFlags AllStatic = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
		public const string BackgroundEntityMark = "__";

		public const string Internal = " internal ";
		public const string Private = " private ";
		public const string Protected = " protected ";
		public const string ProtectedInternal = " protected internal ";
		public const string Public = " public ";
		public const string Sealed = " sealed ";

		private static readonly IDictionary<string, string> correctCollectionTypeNaming = new Dictionary<string, string>
		{
			{"IEnumerable`1", "System.Collections.Generic.IEnumerable" }
		};

		private static readonly IDictionary<string, string> implementTypeNaming = new Dictionary<string, string>
		{
			{ "System.Collections.Generic.IEnumerable", "System.Collections.Generic.List" },
		};

		public const string Generic = "Generic";

		public static string GetCorrectCollectionTypeNaming(string key)
		{
			string value;

			return correctCollectionTypeNaming.TryGetValue(key, out value) ? value : key;
		}

		public static string GetImplementTypeNaming(string key)
		{
			string value;

			//foreach (var ban in banned)
			//{
			//	if (key.Contains(ban))
			//	{
			//		key = key.Remove(0, ban.Length + 1);
			//	}
			//}

			int v = key.IndexOf("<");

			if (v >= 0)
			{
				key = key.Remove(v);
			}

			return implementTypeNaming.TryGetValue(key, out value) ? value : key;
		}

		private static readonly string[] banned =
		{
			"System.Collections.Generic",
			"System.Collections",
		};

		public const string Todo = " TODO ";
	}
}
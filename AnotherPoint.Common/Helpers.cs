using System;
using System.Collections.Generic;
using System.Text;

namespace AnotherPoint.Common
{
	public static class Helpers
	{
		private static readonly IDictionary<string, string> CorrectCollectionTypeNaming;

		private static readonly IDictionary<string, string> ImplementTypeNaming;

		static Helpers()
		{
			Helpers.CorrectCollectionTypeNaming = new Dictionary<string, string>
			{
				{"IEnumerable`1", "System.Collections.Generic.IEnumerable" }
			};

			Helpers.ImplementTypeNaming = new Dictionary<string, string>
			{
				{ "System.Collections.Generic.IEnumerable", "System.Collections.Generic.List" },
			};
		}

		public static string CutNamespaceToInterface(string classNamespace)
		{
			string folderName = Helpers.NameWithoutGeneric(classNamespace);

			int index = folderName.IndexOf(Constant.Interfaces);

			if (index > 0)
			{
				folderName = folderName.Substring(0, index + Constant.Interfaces.Length);
			}

			return folderName;
		}

		public static string NameWithoutGeneric(string str)
		{
			int index = str.IndexOf("<", StringComparison.InvariantCultureIgnoreCase);

			return index > 0 ? str.Substring(0, index) : str;
		}

		public static string GetCorrectCollectionTypeNaming(string key) // TODO unify dicts
		{
			return Helpers.CorrectCollectionTypeNaming.TryGetValue(key, out string value) ? value : key;
		}

		public static string GetDefaultDestinationName(string className)
		{
			return $"{className}Destination";
		}

		public static string GetImplementTypeNaming(string key)
		{
			return Helpers.ImplementTypeNaming.TryGetValue(Helpers.NameWithoutGeneric(key), out string value) ? value : key;
		}
	}
}
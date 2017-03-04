﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
	}
}
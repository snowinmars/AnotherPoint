using System.Collections.Generic;

namespace AnotherPoint.Core
{
	public static class Bag
	{
		static Bag()
		{
			Pocket = new Dictionary<string, string>();
		}

		public static IDictionary<string, string> Pocket { get; private set; }
	}
}
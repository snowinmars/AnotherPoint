using System.Collections.Generic;

namespace AnotherPoint.Core
{
	public static class Bag
	{
		static Bag()
		{
			Bag.Pocket = new Dictionary<string, string>();
		}

		public static IDictionary<string, string> Pocket { get; private set; }
	}
}
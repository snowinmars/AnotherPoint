using AnotherPoint.Entities;
using System.Collections.Generic;

namespace AnotherPoint.Core
{
	public static class Bag
	{
		static Bag()
		{
			Bag.Pocket = new Dictionary<string, MyType>();
		}

		public static IDictionary<string, MyType> Pocket { get; private set; }
	}
}
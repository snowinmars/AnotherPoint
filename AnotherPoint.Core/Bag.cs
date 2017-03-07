using AnotherPoint.Entities;
using System.Collections.Generic;

namespace AnotherPoint.Core
{
	public static class Bag
	{
		static Bag()
		{
			Bag.TypePocket = new Dictionary<string, MyType>();
			Bag.ClassPocket = new Dictionary<string, Class>();
		}

		public static IDictionary<string, MyType> TypePocket { get; private set; }
		public static IDictionary<string, Class> ClassPocket { get; private set; }
	}
}
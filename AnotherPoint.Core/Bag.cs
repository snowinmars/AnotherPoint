using AnotherPoint.Entities;
using System;
using System.Collections.Generic;

namespace AnotherPoint.Core
{
	public static class Bag
	{
		static Bag()
		{
			Bag.MyTypePocket = new Dictionary<string, MyType>();
			Bag.ClassPocket = new Dictionary<string, Class>();
			Bag.TypePocket = new Dictionary<string, Type>();
		}

		public static IDictionary<string, Class> ClassPocket { get; set; }
		public static IDictionary<string, MyType> MyTypePocket { get; set; }
		public static IDictionary<string, Type> TypePocket { get; set; }
	}
}
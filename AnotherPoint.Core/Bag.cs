using AnotherPoint.Entities;
using System;
using System.Collections.Generic;

namespace AnotherPoint.Core
{
	public static class Bag
	{
		static Bag()
		{
			Bag.ClassPocket = new Dictionary<Guid, Class>();
			Bag.MyTypePocket = new Dictionary<string, MyType>();
			Bag.TypePocket = new Dictionary<Guid, Type>();
		}

		public static IDictionary<Guid, Class> ClassPocket { get; set; }
		public static IDictionary<string, MyType> MyTypePocket { get; set; }
		public static IDictionary<Guid, Type> TypePocket { get; set; }
	}
}
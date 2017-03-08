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
			Bag.ValidationPocket = new List<ValidationMethod>();
		}

		public static IDictionary<string, Class> ClassPocket { get; private set; }
		public static IDictionary<string, MyType> MyTypePocket { get; private set; }
		public static IDictionary<string, Type> TypePocket { get; private set; }
		public static IList<ValidationMethod> ValidationPocket { get; private set; }
	}

	public class ValidationMethod
	{
		public ValidationMethod()
		{
			this.DataAnnotationsAttributes = new List<Attribute>();
		}

		public Class ForClass { get; set; }
		public Method ForMethod { get; set; }
		public Argument InputArgument { get; set; }
		public IList<Attribute> DataAnnotationsAttributes { get; private set; }
	}
}
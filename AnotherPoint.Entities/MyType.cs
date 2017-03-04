using System;
using System.Linq;

namespace AnotherPoint.Entities
{
	public class MyType
	{
		public MyType(string fullName)
		{
			FullName = fullName;

			int v = fullName.IndexOf("<");

			if (v >= 0)
			{
				IsGeneric = true;
				Name = fullName.Substring(0, v).Split(new [] {'.'}, StringSplitOptions.RemoveEmptyEntries).Last();
				GenericType = fullName.Substring(v + 1,fullName.Length - 1);
			}
			else
			{
				Name = fullName.Split(new [] {'.'}, StringSplitOptions.RemoveEmptyEntries).Last();
			}
		}

		public string Name { get; set; }
		public string FullName { get; set; }
		public bool IsGeneric { get; set; }
		public string GenericType { get; set; }
	}
}


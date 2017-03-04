using AnotherPoint.Common;
using System;

namespace AnotherPoint.Entities
{
	[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = true)]
	public class CtorBindAttribute : Attribute
	{
		public CtorBindAttribute(CtorBindSettings settings, string name)
		{
			this.Settings = settings;
			this.Name = name;
		}

		public string Name { get; }
		public CtorBindSettings Settings { get; }
	}
}
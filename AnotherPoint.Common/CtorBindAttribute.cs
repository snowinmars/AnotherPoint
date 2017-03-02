using System;

namespace AnotherPoint.Common
{
	[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = true)]
	public class CtorBindAttribute : Attribute
	{
		public CtorBindAttribute(CtorBindSettings settings, string name)
		{
			this.Settings = settings;
			Name = name;
		}

		public string Name { get; }
		public CtorBindSettings Settings { get; }
	}
}
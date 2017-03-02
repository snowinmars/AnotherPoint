using System;

namespace AnotherPoint.Common
{
	public enum CtorBindSettings
	{
		Exact = 1,
		New = 2,
	}

	[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = true)]
	public class CtorBindAttribute : Attribute
	{
		public CtorBindAttribute(CtorBindSettings settings, string name)
		{
			this.Settings = settings;
			Name = name;
		}

		public CtorBindSettings Settings { get; }
		public string Name { get; }
	}
}
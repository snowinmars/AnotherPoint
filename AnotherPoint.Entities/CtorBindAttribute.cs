using AnotherPoint.Common;
using System;
using System.Text;

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

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append($"{this.Name} to {this.Settings}");

			return sb.ToString();
		}
	}
}
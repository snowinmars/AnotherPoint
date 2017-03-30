using System;

namespace AnotherPoint.Entities
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true)]
	public class InsertUsingAttribute : AnotherPointAttribute
	{
		public InsertUsingAttribute(string @using)
		{
			this.Using = @using;
		}

		public string Using { get; }

		public override bool Equals(object obj)
		{
			// ReSharper disable once ConditionIsAlwaysTrueOrFalse : it depends from compiler, google about callvirt and call CLR instructions
			if (this == null)
			{
				return false;
			}

			InsertUsingAttribute insertUsingAttribute = obj as InsertUsingAttribute;

			if (insertUsingAttribute == null)
			{
				return false;
			}

			return this.Equals(insertUsingAttribute);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (base.GetHashCode() * 397) ^ (this.Using?.GetHashCode() ?? 0);
			}
		}

		public bool Equals(InsertUsingAttribute other)
			=> this.Using == other.Using;
	}
}
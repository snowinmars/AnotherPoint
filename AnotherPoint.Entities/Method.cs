using AnotherPoint.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnotherPoint.Entities
{
	public class Method : AnotherPointObject
	{
		public Method(string name, string fullNameOfReturnType)
		{
			this.Name = name;
			this.AccessModifyer = AccessModifyer.Public;

			this.Arguments = new List<Argument>();
			this.ReturnType = new MyType(fullNameOfReturnType);

			this.AttributesForBodyGeneration = new List<Attribute>();
			this.EntityPurposePair = new EntityPurposePair("", "");

			this.AdditionalBody = "";
		}

		public string AdditionalBody { get; set; }

		public AccessModifyer AccessModifyer { get; set; }

		public IList<Argument> Arguments { get; }

		public IList<Attribute> AttributesForBodyGeneration { get; set; }

		public EntityPurposePair EntityPurposePair { get; set; }

		public string Name { get; set; }

		public MyType ReturnType { get; set; }

		public override bool Equals(object obj)
		{
			// ReSharper disable once ConditionIsAlwaysTrueOrFalse : it depends from compiler, google about callvirt and call CLR instructions
			if (this == null)
			{
				return false;
			}

			Method method = obj as Method;

			if (method == null)
			{
				return false;
			}

			return this.Equals(method);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = this.AdditionalBody?.GetHashCode() ?? 0;
				hashCode = (hashCode * 397) ^ (int) this.AccessModifyer;
				hashCode = (hashCode * 397) ^ (this.Arguments?.GetHashCode() ?? 0);
				hashCode = (hashCode * 397) ^ (this.AttributesForBodyGeneration?.GetHashCode() ?? 0);
				hashCode = (hashCode * 397) ^ (this.EntityPurposePair?.GetHashCode() ?? 0);
				hashCode = (hashCode * 397) ^ (this.Name?.GetHashCode() ?? 0);
				hashCode = (hashCode * 397) ^ (this.ReturnType?.GetHashCode() ?? 0);
				return hashCode;
			}
		}

		public bool Equals(Method other)
			=> this.Name == other.Name &&
			   this.AccessModifyer == other.AccessModifyer &&
			   this.Arguments.OrderBy(a => a).SequenceEqual(other.Arguments.OrderBy(a => a)) &&
			   this.ReturnType.Equals(other.ReturnType) &&
			   this.AttributesForBodyGeneration.OrderBy(a => a).SequenceEqual(other.AttributesForBodyGeneration.OrderBy(a => a)) &&
			   this.EntityPurposePair.Equals(other.EntityPurposePair);

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append($"{this.ReturnType.Name} {this.Name} ({this.Arguments.Count})");

			return sb.ToString();
		}
	}
}
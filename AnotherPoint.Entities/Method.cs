using AnotherPoint.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnotherPoint.Entities
{
	public class Method
	{
		public Method(string name, string fullNameOfReturnType)
		{
			this.Name = name;
			this.AccessModifyer = AccessModifyer.Public;

			this.Arguments = new List<Argument>();
			this.ReturnType = new MyType(fullNameOfReturnType);

			this.AttributesForBodyGeneration = new List<Attribute>();
			this.EntityPurposePair = null;
		}

		public AccessModifyer AccessModifyer { get; set; }
		public IList<Argument> Arguments { get; }
		public IList<Attribute> AttributesForBodyGeneration { get; set; }
		public EntityPurposePair EntityPurposePair { get; set; }
		public string Name { get; set; }
		public MyType ReturnType { get; set; }

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append($"{this.ReturnType.Name} {this.Name} ({this.Arguments.Count})");

			return sb.ToString();
		}
	}
}
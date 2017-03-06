using AnotherPoint.Common;
using System.Collections.Generic;
using System.Text;

namespace AnotherPoint.Entities
{
	public class Ctor
	{
		public Ctor(string fullTypeName)
		{
			this.Type = new MyType(fullTypeName);
			this.AccessModifyer = AccessModifyer.Public;
			this.ArgumentCollection = new List<Argument>();

			this.IsCtorForInject = false;
		}

		public AccessModifyer AccessModifyer { get; set; }

		public IList<Argument> ArgumentCollection { get; }

		public bool IsCtorForInject { get; set; }

		public MyType Type { get; set; }

		public bool IsDefaultCtor(bool ignoreAccessModifyer = true)
		{
			if (ignoreAccessModifyer)
			{
				return this.ArgumentCollection.Count == 0;
			}

			return this.ArgumentCollection.Count == 0 && this.AccessModifyer.HasFlag(AccessModifyer.Public);
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append($"{this.AccessModifyer.AsString()} {this.Type.Name} ({this.ArgumentCollection.Count} args)");

			return sb.ToString();
		}
	}
}
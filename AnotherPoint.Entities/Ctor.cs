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

		public bool IsDefaultCtor()
			=> this.ArgumentCollection.Count == 0;

		public bool IsCtorForInject { get; set; }
		public AccessModifyer AccessModifyer { get; set; }
		public IList<Argument> ArgumentCollection { get; }
		public MyType Type { get; set; }

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append($"{this.AccessModifyer.AsString()} {this.Type.Name} ({this.ArgumentCollection.Count} args)");

			return sb.ToString();
		}
	}
}
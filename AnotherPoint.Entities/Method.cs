using System.Collections.Generic;
using System.Linq;
using AnotherPoint.Common;
using System.Text;
using Microsoft.SqlServer.Server;

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
		}

		public AccessModifyer AccessModifyer { get; set; }
		public string Name { get; set; }
		public IList<Argument> Arguments { get; }
		public MyType ReturnType { get; set; }

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append($"{this.ReturnType.Name} {this.Name} ({this.Arguments.Count})");

			return sb.ToString();
		}
	}
}
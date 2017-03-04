using AnotherPoint.Common;
using System.Collections.Generic;
using System.Text;

namespace AnotherPoint.Entities
{
	public class Class
	{
		public Class(string fullTypeName)
		{
			this.Type = new MyType(fullTypeName);

			this.Ctors = new List<Ctor>();
			this.Fields = new List<Field>();
			this.Properties = new List<Property>();

			this.AccessModifyer = AccessModifyer.Public;
		}

		public AccessModifyer AccessModifyer { get; set; }

		public IList<Ctor> Ctors { get; private set; }

		public IList<Field> Fields { get; private set; }

		public string Name
		{
			get { return this.Type.Name; }
			set { this.Type.Name = value; }
		}

		public string Namespace
		{
			get { return this.Type.Namespace; }
			set { this.Type.Namespace = value; }
		}

		public IList<Property> Properties { get; private set; }
		public MyType Type { get; set; }

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(this.Type.Name);

			if (this.Type.IsGeneric)
			{
				sb.Append("<...>");
			}

			sb.Append($" {this.Name}");

			return sb.ToString();
		}
	}
}
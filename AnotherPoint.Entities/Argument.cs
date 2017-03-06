using AnotherPoint.Common;
using System.Text;

namespace AnotherPoint.Entities
{
	public class Argument
	{
		public Argument(string name, string typeName, BindSettings bindAttribute)
		{
			this.Name = name;
			this.BindAttribute = bindAttribute;

			this.Type = new MyType(typeName);
		}

		public BindSettings BindAttribute { get; set; }

		public string Name { get; set; }

		public MyType Type { get; set; }

		public string GetFullTypeName()
		{
			if (this.Type.IsGeneric.HasValue && this.Type.IsGeneric.Value)
			{
				StringBuilder sb = new StringBuilder();

				sb.Append(this.Type.FullName);
				sb.Append("<");

				string s = string.Join(",", this.Type.GenericTypes);
				sb.Append(s);

				sb.Append(">");

				return sb.ToString();
			}

			return this.Type.FullName;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append($"{this.Type.Name} {this.Name}");

			return sb.ToString();
		}
	}
}
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

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append($"{this.Type.Name} {this.Name}");

			return sb.ToString();
		}
	}
}
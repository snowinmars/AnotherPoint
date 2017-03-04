using AnotherPoint.Common;
using System.Text;

namespace AnotherPoint.Entities
{
	public class Field
	{
		public Field(string name, string typeName)
		{
			this.Name = name;

			this.Type = new MyType(typeName);
		}

		public AccessModifyer AccessModifyer { get; set; }
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
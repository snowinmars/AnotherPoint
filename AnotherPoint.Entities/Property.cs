using AnotherPoint.Common;
using System.Text;

namespace AnotherPoint.Entities
{
	public class Property
	{
		public Property(string name, string typeName)
		{
			this.Name = name;

			this.GetMethod = new Method(Constant.Get);
			this.SetMethod = new Method(Constant.Set);
			this.Type = new MyType(typeName);
		}

		public AccessModifyer AccessModifyer
		{
			get { return this.GetMethod.AccessModifyer; }
			set { this.GetMethod.AccessModifyer = value; }
		}

		public Method GetMethod { get; set; }
		public string Name { get; set; }
		public Method SetMethod { get; set; }
		public MyType Type { get; set; }

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append($"{this.Type.Name} {this.Name}");

			return sb.ToString();
		}
	}
}
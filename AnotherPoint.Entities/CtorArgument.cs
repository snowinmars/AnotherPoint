using AnotherPoint.Common;

namespace AnotherPoint.Entities
{
	public class CtorArgument
	{
		public CtorArgument(string name, string typeName, CtorBindSettings bindAttribute)
		{
			this.Name = name;
			this.BindAttribute = bindAttribute;

			this.Type = new MyType(typeName);
		}

		public CtorBindSettings BindAttribute { get; set; }
		public string Name { get; set; }
		public MyType Type { get; set; }
	}
}
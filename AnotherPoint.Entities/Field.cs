using AnotherPoint.Common;

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
	}
}
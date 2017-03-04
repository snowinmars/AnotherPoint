
using AnotherPoint.Common;

namespace AnotherPoint.Entities
{
	public class Field
	{
		public Field(string name, string typeName)
		{
			Name = name;

			Type = new MyType(typeName);
		}

		public string Name { get; set; }
		public MyType Type { get; set; }
		public AccessModifyer AccessModifyer { get; set; }
	}
}

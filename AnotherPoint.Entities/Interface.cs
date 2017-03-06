using System.Collections.Generic;

namespace AnotherPoint.Entities
{
	public class Interface
	{
		public Interface(string fullTypeName)
		{
			this.Type = new MyType(fullTypeName);

			this.Methods = new List<Method>();
		}

		public string FullName
		{
			get { return this.Type.FullName; }
			set { this.Type.FullName = value; }
		}

		public IList<Method> Methods { get; }

		public string Name
		{
			get { return this.Type.Name; }
			set { this.Type.Name = value; }
		}

		public MyType Type { get; set; }
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnotherPoint.Common;

namespace AnotherPoint.Entities
{
	public class CtorArgument
	{
		public CtorArgument(string name, string typeName, CtorBindSettings bindAttribute)
		{
			Name = name;
			BindAttribute = bindAttribute;

			Type = new MyType(typeName);
		}

		public MyType Type { get; set; }
		public string Name { get; set; }
		public CtorBindSettings BindAttribute { get; set; }
	}
}

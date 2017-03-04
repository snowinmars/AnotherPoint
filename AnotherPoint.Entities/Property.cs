using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnotherPoint.Common;

namespace AnotherPoint.Entities
{
    public class Property
    {
	    public Property(string name, string typeName)
	    {
		    Name = name;

			GetMethod = new Method(Constant.Get);
			SetMethod = new Method(Constant.Set);
			Type = new MyType(typeName);
	    }

		public Method GetMethod { get; set; }
	    public Method SetMethod { get; set; }
	    public string Name { get; set; }
	    public MyType Type { get; set; }
	}
}

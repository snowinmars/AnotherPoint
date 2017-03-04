using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnotherPoint.Common;

namespace AnotherPoint.Entities
{
	public class Ctor
	{
		public Ctor(string fullTypeName)
		{
			Type = new MyType(fullTypeName);
			ArgumentCollection = new List<CtorArgument>();
		}

		public MyType Type { get; set; }
		public AccessModifyer AccessModifyer { get; set; }
		public IList<CtorArgument> ArgumentCollection { get; private set; }

		
	}
}

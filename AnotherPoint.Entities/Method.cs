using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnotherPoint.Common;

namespace AnotherPoint.Entities
{
	public class Method
	{
		public Method(string name)
		{
			Name = name;
			AccessModifyer = AccessModifyer.Public;
		}

		public string Name { get; set; }
		public AccessModifyer AccessModifyer { get; set; }
	}
}

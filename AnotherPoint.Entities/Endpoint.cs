using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnotherPoint.Entities
{
	public class Endpoint
	{
		public Endpoint()
		{
			this.BLLInterfaces = new List<Interface>();
			this.DAOInterfaces = new List<Interface>();
		}

		public string AppName { get; set; }
		public Class EntityClass { get; set; }
		public Class BLLClass { get; set; }
		public Class DAOClass { get; set; }
		public List<Interface> BLLInterfaces { get; private set; }
		public List<Interface> DAOInterfaces { get; private set; }
		public Class CommonClass { get; set; }
	}
}

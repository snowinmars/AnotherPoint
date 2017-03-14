using System.Collections.Generic;

namespace AnotherPoint.Entities
{
	public class Endpoint
	{
		public Endpoint()
		{
			this.BLLInterfaces = new List<Interface>();
			this.DAOInterfaces = new List<Interface>();
		}

		public IEnumerable<Class> Classes
		{
			get
			{
				yield return this.CommonClass;
				yield return this.EntityClass;
				yield return this.BLLClass;
				yield return this.DAOClass;
			}
		}

		public IEnumerable<Interface> Interfaces
		{
			get
			{
				foreach (var bllInterface in this.BLLInterfaces)
				{
					yield return bllInterface;
				}

				foreach (var daoInterface in this.DAOInterfaces)
				{
					yield return daoInterface;
				}
			}
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
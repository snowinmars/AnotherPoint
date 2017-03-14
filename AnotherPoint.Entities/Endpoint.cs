using System.Collections.Generic;

namespace AnotherPoint.Entities
{
	public class Endpoint
	{
		public Endpoint()
		{
			this.BllInterfaces = new List<Interface>();
			this.DaoInterfaces = new List<Interface>();
		}

		public IEnumerable<Class> Classes
		{
			get
			{
				yield return this.CommonClass;
				yield return this.EntityClass;
				yield return this.BllClass;
				yield return this.DaoClass;
			}
		}

		public IEnumerable<Interface> Interfaces
		{
			get
			{
				foreach (var bllInterface in this.BllInterfaces)
				{
					yield return bllInterface;
				}

				foreach (var daoInterface in this.DaoInterfaces)
				{
					yield return daoInterface;
				}
			}
		}

		public string AppName { get; set; }
		public Class EntityClass { get; set; }
		public Class BllClass { get; set; }
		public Class DaoClass { get; set; }
		public List<Interface> BllInterfaces { get; private set; }
		public List<Interface> DaoInterfaces { get; private set; }
		public Class CommonClass { get; set; }
	}
}
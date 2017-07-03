using System.Collections.Generic;

namespace AnotherPoint.Entities
{
	public class Endpoint : AnotherPointObject
	{
		public Endpoint()
		{
			this.BllInterfaces = new List<Interface>();
			this.DaoInterfaces = new List<Interface>();
		}

		public string AppName { get; set; }

		public Class BllClass { get; set; }

		public List<Interface> BllInterfaces { get; }

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

		public Class CommonClass { get; set; }

		public Class DaoClass { get; set; }

		public List<Interface> DaoInterfaces { get; }

		public Class EntityClass { get; set; }

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

		public override string ToString()
		{
			return $"{this.AppName}: {this.EntityClass.Name}";
		}
	}
}
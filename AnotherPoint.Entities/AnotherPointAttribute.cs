using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnotherPoint.Entities
{
	public abstract class AnotherPointAttribute : Attribute
	{
		protected AnotherPointAttribute()
		{
			this.Id = Guid.NewGuid();
		}

		public Guid Id { get; set; }
	}
}

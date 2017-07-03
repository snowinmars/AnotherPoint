using System;

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
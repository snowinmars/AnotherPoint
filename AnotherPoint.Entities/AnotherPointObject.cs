using System;

namespace AnotherPoint.Entities
{
	public abstract class AnotherPointObject
	{
		protected AnotherPointObject()
		{
			this.Id = Guid.NewGuid();
		}

		public Guid Id { get; set; }
	}
}
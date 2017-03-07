namespace AnotherPoint.Entities
{
	public class EntityPurposePair
	{
		public EntityPurposePair(string entity, string purpose)
		{
			this.Entity = entity;
			this.Purpose = purpose;
		}

		public bool IsEmpty()
		{
			return string.IsNullOrWhiteSpace(this.Entity) || // or
				string.IsNullOrWhiteSpace(this.Purpose);
		}

		public string Entity { get; set; }
		public string Purpose { get; set; }

		public string Both
		{
			get { return this.Entity + this.Purpose; }
		}
	}
}
namespace AnotherPoint.Entities
{
	public class EntityPurposePair : AnotherPointObject
	{
		public EntityPurposePair(string entity, string purpose)
		{
			this.Entity = entity;
			this.Purpose = purpose;
		}

		public string Both
		{
			get { return this.Entity + this.Purpose; }
		}

		public string Entity { get; set; }

		public string Purpose { get; set; }

		public override bool Equals(object obj)
		{
			// ReSharper disable once ConditionIsAlwaysTrueOrFalse : it depends from compiler, google about callvirt and call CLR instructions
			if (this == null)
			{
				return false;
			}

			EntityPurposePair entityPurposePair = obj as EntityPurposePair;

			if (entityPurposePair == null)
			{
				return false;
			}

			return this.Equals(entityPurposePair);
		}

		public bool Equals(EntityPurposePair other)
			=> this.Entity == other.Entity &&
			   this.Purpose == other.Purpose &&
			   this.Both == other.Both;

		public override int GetHashCode()
		{
			unchecked
			{
				return ((this.Entity?.GetHashCode() ?? 0) * 397) ^ (this.Purpose?.GetHashCode() ?? 0);
			}
		}

		public bool IsEmpty()
		{
			return string.IsNullOrWhiteSpace(this.Entity) || 
				string.IsNullOrWhiteSpace(this.Purpose);
		}
	}
}
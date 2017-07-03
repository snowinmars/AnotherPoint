namespace AnotherPoint.Entities
{
	public class ClassImplAttribute : AnotherPointAttribute
	{
		public ClassImplAttribute(string destinationTypeName = null)
		{
			this.DestinationTypeName = destinationTypeName;
			this.IsEndPoint = destinationTypeName != null;
		}

		public string DestinationTypeName { get; }
		public bool IsEndPoint { get; }

		public override bool Equals(object obj)
		{
			// ReSharper disable once ConditionIsAlwaysTrueOrFalse : it depends from compiler, google about callvirt and call CLR instructions
			if (this == null)
			{
				return false;
			}

			ClassImplAttribute classImplAttribute = obj as ClassImplAttribute;

			if (classImplAttribute == null)
			{
				return false;
			}

			return this.Equals(classImplAttribute);
		}

		public bool Equals(ClassImplAttribute other)
			=> this.IsEndPoint == other.IsEndPoint &&
			   this.DestinationTypeName == other.DestinationTypeName;

		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = base.GetHashCode();
				hashCode = (hashCode * 397) ^ (this.DestinationTypeName?.GetHashCode() ?? 0);
				hashCode = (hashCode * 397) ^ this.IsEndPoint.GetHashCode();
				return hashCode;
			}
		}

		public override string ToString()
		{
			return $"Is {(this.IsEndPoint ? "" : "not")} endpoint";
		}
	}
}
using System;

namespace AnotherPoint.Entities
{
	public class ClassImplAttribute : Attribute
	{
		public ClassImplAttribute(string destinationTypeName = null)
		{
			this.DestinationTypeName = destinationTypeName;
			this.IsEndPoint = destinationTypeName != null;
		}

		public string DestinationTypeName { get; }
		public bool IsEndPoint { get; }

		public override string ToString()
		{
			return $"Is {(this.IsEndPoint ? "" : "not")} endpoint";
		}
	}
}
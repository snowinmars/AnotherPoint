using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnotherPoint.Entities
{
	public class ClassImplAttribute : Attribute
	{
		public string DestinationTypeName { get; }
		public bool IsEndPoint { get; }

		public ClassImplAttribute(string destinationTypeName = null)
		{
			this.DestinationTypeName = destinationTypeName;
			this.IsEndPoint = destinationTypeName != null;
		}

		public override string ToString()
		{
			return $"Is {(this.IsEndPoint ? "" : "not")} endpoint";
		}
	}
}

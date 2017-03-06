using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnotherPoint.Entities.MethodImpl
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class MethodImpl_SendMeToAttribute : Attribute
	{
		public MethodImpl_SendMeToAttribute(string destination)
		{
			this.Destination = destination;
		}

		public string Destination { get; }

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			if (this.Destination != null)
			{
				sb.Append($"Send to {this.Destination}");
			}

			return sb.ToString();
		}
	}
}

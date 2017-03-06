using System;
using System.Text;

namespace AnotherPoint.Entities.MethodImpl
{
	[AttributeUsage(AttributeTargets.Method)]
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
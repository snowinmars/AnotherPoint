using System;
using System.Collections.Generic;
using System.Text;

namespace AnotherPoint.Entities.MethodImpl
{
	public abstract class MethodImpl
	{
		private MethodImpl()
		{
		}

		[AttributeUsage(AttributeTargets.Method)]
		public class SendMeToAttribute : Attribute
		{
			public SendMeToAttribute(string destination)
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

		[AttributeUsage(AttributeTargets.Method)]
		public class ShutMeUpAttribute : Attribute
		{
		}

		[AttributeUsage(AttributeTargets.Method)]
		public class ValidateAttribute : Attribute
		{
			public ValidateAttribute(string[] namesOfInputParametersToValidate)
			{
				this.NamesOfInputParametersToValidate = namesOfInputParametersToValidate;
			}

			public IEnumerable<string> NamesOfInputParametersToValidate { get; }

			public override string ToString()
			{
				StringBuilder sb = new StringBuilder();

				if (this.NamesOfInputParametersToValidate != null)
				{
					sb.Append($"Validate {string.Join(",", this.NamesOfInputParametersToValidate)}");
				}

				return sb.ToString();
			}
		}

		[AttributeUsage(AttributeTargets.Method)]
		public class ToSqlAttribute : Attribute
		{
		}
	}
}
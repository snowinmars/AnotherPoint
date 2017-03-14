using System;
using System.Collections.Generic;
using System.Linq;
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
			public SendMeToAttribute(string destination, string @from)
			{
				this.Destination = destination;
				this.From = @from;
			}

			public string Destination { get; }
			public string From { get; }

			public override bool Equals(object obj)
			{
				// ReSharper disable once ConditionIsAlwaysTrueOrFalse : it depends from compiler, google about callvirt and call CLR instructions
				if (this == null)
				{
					return false;
				}

				SendMeToAttribute sendMeToAttribute = obj as SendMeToAttribute;

				if (sendMeToAttribute == null)
				{
					return false;
				}

				return this.Equals(sendMeToAttribute);
			}

			public bool Equals(SendMeToAttribute other)
				=> this.Destination == other.Destination &&
				this.From == other.From;

			public override string ToString()
			{
				StringBuilder sb = new StringBuilder();

				if (this.Destination != null)
				{
					sb.Append($"Send from {this.From} to {this.Destination}");
				}

				return sb.ToString();
			}
		}

		[AttributeUsage(AttributeTargets.Method)]
		public class ShutMeUpAttribute : Attribute
		{
		}

		[AttributeUsage(AttributeTargets.Method)]
		public class ToSqlAttribute : Attribute
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

			public override bool Equals(object obj)
			{
				// ReSharper disable once ConditionIsAlwaysTrueOrFalse : it depends from compiler, google about callvirt and call CLR instructions
				if (this == null)
				{
					return false;
				}

				ValidateAttribute validateAttribute = obj as ValidateAttribute;

				if (validateAttribute == null)
				{
					return false;
				}

				return this.Equals(validateAttribute);
			}

			public bool Equals(ValidateAttribute other)
				=>
					this.NamesOfInputParametersToValidate.OrderBy(a => a)
						.SequenceEqual(other.NamesOfInputParametersToValidate.OrderBy(a => a));

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
	}
}
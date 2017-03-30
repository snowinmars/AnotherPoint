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
		public class SendMeToAttribute : AnotherPointAttribute
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

			public override int GetHashCode()
			{
				unchecked
				{
					int hashCode = base.GetHashCode();
					hashCode = (hashCode * 397) ^ (this.Destination?.GetHashCode() ?? 0);
					hashCode = (hashCode * 397) ^ (this.From?.GetHashCode() ?? 0);
					return hashCode;
				}
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
		public class ShutMeUpAttribute : AnotherPointAttribute
		{
		}

		[AttributeUsage(AttributeTargets.Method)]
		public class ToSqlAttribute : AnotherPointAttribute
		{
		}

		[AttributeUsage(AttributeTargets.Method)]
		public class ValidateAttribute : AnotherPointAttribute
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

			public override int GetHashCode()
			{
				unchecked
				{
					return (base.GetHashCode() * 397) ^ (this.NamesOfInputParametersToValidate?.GetHashCode() ?? 0);
				}
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
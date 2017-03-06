using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnotherPoint.Entities.MethodImpl
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class MethodImpl_ValidateAttribute : Attribute
	{
		public MethodImpl_ValidateAttribute(string[] namesOfInputParametersToValidate)
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
}

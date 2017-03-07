using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnotherPoint.Entities
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class InsertUsingAttribute : Attribute
	{
		public string Using { get; }

		public InsertUsingAttribute(string @using)
		{
			this.Using = @using;
		}
	}
}

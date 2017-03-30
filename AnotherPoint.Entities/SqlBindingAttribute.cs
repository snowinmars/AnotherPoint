using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnotherPoint.Entities
{
	public enum SqlBindingType
	{
		None = 0,
		OneToMany = 1,
		ManyToMany = 2,
	}

	public class SqlBindingAttribute : AnotherPointAttribute
	{
		public SqlBindingType SqlBindingType { get; }

		public SqlBindingAttribute(SqlBindingType sqlBindingType)
		{
			this.SqlBindingType = sqlBindingType;
		}
	}
}

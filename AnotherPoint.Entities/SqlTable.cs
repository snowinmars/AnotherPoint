using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnotherPoint.Entities
{
	public class SqlTable
	{
		public SqlTable(string name)
		{
			this.Name = name;
			//this.ForEntity = forEntity;

			this.Columns = new List<SqlTableColumn>();
		}

		public string Name { get; set; }
		//public MyType ForEntity { get; set; }
		private IList<SqlTableColumn> Columns { get; }

		public bool TryAddColumn(SqlTableColumn column)
		{
			if (this.Columns.Contains(column))
			{
				return false;
			}

			this.Columns.Add(column);
			return true;
		}

		public override bool Equals(object obj)
		{
			SqlTable s = obj as SqlTable;

			return s != null && this.Equals(s);
		}

		protected bool Equals(SqlTable other)
		{
			return string.Equals(this.Name, other.Name) &&
				//object.Equals(this.ForEntity, other.ForEntity) &&
				object.Equals(this.Columns, other.Columns);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (this.Name != null ? this.Name.GetHashCode() : 0);
				//hashCode = (hashCode * 397) ^ (this.ForEntity != null ? this.ForEntity.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (this.Columns != null ? this.Columns.GetHashCode() : 0);
				return hashCode;
			}
		}
	}
}


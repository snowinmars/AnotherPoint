using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnotherPoint.Entities
{
	public class SqlTableColumn
	{
		public SqlTableColumn(string name, string type, bool canBeNull)
		{
			this.Name = name;
			this.Type = type;
			this.CanBeNull = canBeNull;
		}

		public SqlTableColumn(string name, string type) : this(name, type, false)
		{
			
		}

		public string Name { get; set; }
		public string Type { get; set; }
		public bool CanBeNull { get; set; }

		public override bool Equals(object obj)
		{
			SqlTableColumn s = obj as SqlTableColumn;

			return s != null && this.Equals(s);
		}

		protected bool Equals(SqlTableColumn other)
		{
			return string.Equals(this.Name, other.Name) && 
				string.Equals(this.Type, other.Type) &&
				this.CanBeNull == other.CanBeNull;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (this.Name != null ? this.Name.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (this.Type != null ? this.Type.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ this.CanBeNull.GetHashCode();
				return hashCode;
			}
		}

		public override string ToString()
		{
			return $"{this.Name} {this.Type}";
		}
	}
}

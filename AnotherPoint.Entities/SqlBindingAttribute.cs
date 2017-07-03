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
		public SqlBindingAttribute(SqlBindingType sqlBindingType)
		{
			this.SqlBindingType = sqlBindingType;
		}

		public SqlBindingType SqlBindingType { get; }
	}
}
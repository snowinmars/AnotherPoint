using AnotherPoint.Common;
using System.Text;

namespace AnotherPoint.Entities
{
	public class Field
	{
		public Field(string name, string typeName)
		{
			this.Name = name;
			this.AccessModifyer = AccessModifyer.Public;

			this.Type = new MyType(typeName);
		}

		public AccessModifyer AccessModifyer { get; set; }

		public string Name { get; set; }

		public MyType Type { get; set; }

		public override bool Equals(object obj)
		{
			// ReSharper disable once ConditionIsAlwaysTrueOrFalse : it depends from compiler, google about callvirt and call CLR instructions
			if (this == null)
			{
				return false;
			}

			Field field = obj as Field;

			if (field == null)
			{
				return false;
			}

			return this.Equals(field);
		}

		public bool Equals(Field other)
			=> this.Type.Equals(other.Type) &&
			   this.AccessModifyer == other.AccessModifyer &&
			   this.Name == other.Name;

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append($"{this.Type.Name} {this.Name}");

			return sb.ToString();
		}
	}
}
using AnotherPoint.Common;
using System.Text;

namespace AnotherPoint.Entities
{
	public class Property : AnotherPointObject
	{
		public Property(string name, string typeName)
		{
			this.Name = name;

			this.GetMethod = new Method(Constant.Get, typeName);
			this.SetMethod = new Method(Constant.Set, typeName);
			this.Type = new MyType(typeName);
		}

		public AccessModifyer AccessModifyer
		{
			get => this.GetMethod.AccessModifyer;
			set => this.GetMethod.AccessModifyer = value;
		}

		public Method GetMethod { get; set; }
		public string Name { get; set; }
		public Method SetMethod { get; set; }
		public MyType Type { get; set; }

		public override bool Equals(object obj)
		{
			// ReSharper disable once ConditionIsAlwaysTrueOrFalse : it depends from compiler, google about callvirt and call CLR instructions
			if (this == null)
			{
				return false;
			}

			Property property = obj as Property;

			if (property == null)
			{
				return false;
			}

			return this.Equals(property);
		}

		public bool Equals(Property other)
			=> this.Name == other.Name &&
			   this.AccessModifyer == other.AccessModifyer &&
			   this.Type.Equals(other.Type) &&
			   this.GetMethod.Equals(other.GetMethod) &&
			   this.SetMethod.Equals(other.SetMethod);

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = this.GetMethod?.GetHashCode() ?? 0;
				hashCode = (hashCode * 397) ^ (this.Name?.GetHashCode() ?? 0);
				hashCode = (hashCode * 397) ^ (this.SetMethod?.GetHashCode() ?? 0);
				hashCode = (hashCode * 397) ^ (this.Type?.GetHashCode() ?? 0);
				return hashCode;
			}
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append($"{this.Type.Name} {this.Name}");

			return sb.ToString();
		}
	}
}
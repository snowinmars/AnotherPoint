using AnotherPoint.Common;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnotherPoint.Entities
{
	public class Class
	{
		public Class(string fullTypeName)
		{
			this.Type = new MyType(fullTypeName);

			this.Ctors = new List<Ctor>();
			this.Fields = new List<Field>();
			this.Usings = new List<string>();
			this.Properties = new List<Property>();
			this.ImplementedInterfaces = new List<Interface>();
			this.Methods = new List<Method>();
			this.Constants = new List<Field>();

			this.AccessModifyer = AccessModifyer.Public;

			this.EntityPurposePair = new EntityPurposePair("", "");
			this.DestinationTypeName = "";
			this.IsEndpoint = false;
		}

		public AccessModifyer AccessModifyer { get; set; }
		public IList<Field> Constants { get; private set; }
		public IList<Ctor> Ctors { get; private set; }
		public string DestinationTypeName { get; set; }
		public EntityPurposePair EntityPurposePair { get; set; }
		public IList<Field> Fields { get; private set; }
		public IList<Interface> ImplementedInterfaces { get; }
		public bool IsEndpoint { get; set; }
		public IList<Method> Methods { get; }

		public string Name
		{
			get
			{
				if (string.IsNullOrWhiteSpace(this.EntityPurposePair.Purpose) ||
					string.IsNullOrWhiteSpace(this.EntityPurposePair.Entity))
				{
					return this.Type.Name;
				}

				return this.EntityPurposePair.Both;
			}
		}

		public string FullName
		{
			get { return $"{this.Namespace}.{this.Name}"; }
		}

		public string Namespace
		{
			get { return this.Type.Namespace; }
			set { this.Type.Namespace = value; }
		}

		public IList<Property> Properties { get; private set; }
		public MyType Type { get; set; }
		public IList<string> Usings { get; private set; }

		public override bool Equals(object obj)
		{
			// ReSharper disable once ConditionIsAlwaysTrueOrFalse : it depends from compiler, google about callvirt and call CLR instructions
			if (this == null)
			{
				return false;
			}

			Class @class = obj as Class;

			if (@class == null)
			{
				return false;
			}

			return this.Equals(@class);
		}

		public bool Equals(Class other)
			=>
				this.AccessModifyer == other.AccessModifyer &&
				this.DestinationTypeName == other.DestinationTypeName &&
				this.EntityPurposePair.Equals(other.EntityPurposePair) &&
				this.IsEndpoint == other.IsEndpoint &&
				this.Name == other.Name &&
				this.Namespace == other.Namespace &&
				this.Type.Equals(other.Type) &&

				this.Constants.OrderBy(a => a).SequenceEqual(other.Constants.OrderBy(a => a)) &&
				this.Ctors.OrderBy(a => a).SequenceEqual(other.Ctors.OrderBy(a => a)) &&
				this.Fields.OrderBy(a => a).SequenceEqual(other.Fields.OrderBy(a => a)) &&
				this.ImplementedInterfaces.OrderBy(a => a).SequenceEqual(other.ImplementedInterfaces.OrderBy(a => a)) &&
				this.Methods.OrderBy(a => a).SequenceEqual(other.Methods.OrderBy(a => a)) &&
				this.Properties.OrderBy(a => a).SequenceEqual(other.Properties.OrderBy(a => a)) &&
				this.Usings.OrderBy(a => a).SequenceEqual(other.Usings.OrderBy(a => a));

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(this.Type.Name);

			if (this.Type.IsGeneric.HasValue && this.Type.IsGeneric.Value)
			{
				sb.Append("<...>");
			}

			sb.Append($" {this.Name}");

			return sb.ToString();
		}
	}
}
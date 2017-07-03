using AnotherPoint.Common;
using System.Collections.Generic;
using System.Linq;

namespace AnotherPoint.Entities
{
	public class Interface : AnotherPointObject
	{
		public Interface(string fullTypeName)
		{
			this.Type = new MyType(fullTypeName);

			this.AccessModifyer = AccessModifyer.Public;

			this.ImplementedInterfaces = new List<Interface>();
			this.References = new List<string>();
			this.OverrideGenericTypes = new Dictionary<string, string>();

			this.Usings = new List<string>();
			this.Methods = new List<Method>();
		}

		public AccessModifyer AccessModifyer { get; set; }

		public string FullName
		{
			get { return this.Type.FullName; }
			set { this.Type.FullName = value; }
		}

		public IList<Interface> ImplementedInterfaces { get; private set; }
		public IList<Method> Methods { get; }

		public string Name
		{
			get { return this.Type.Name; }
			set { this.Type.Name = value; }
		}

		public string Namespace
		{
			get { return this.Type.Namespace; }
			set { this.Type.Namespace = value; }
		}

		public IDictionary<string, string> OverrideGenericTypes { get; private set; }
		public IEnumerable<string> References { get; private set; }
		public MyType Type { get; set; }
		public IList<string> Usings { get; }

		public override bool Equals(object obj)
		{
			// ReSharper disable once ConditionIsAlwaysTrueOrFalse : it depends from compiler, google about callvirt and call CLR instructions
			if (this == null)
			{
				return false;
			}

			Interface @interface = obj as Interface;

			if (@interface == null)
			{
				return false;
			}

			return this.Equals(@interface);
		}

		public bool Equals(Interface other)
			=> this.FullName == other.FullName &&
			   this.Methods.OrderBy(a => a).SequenceEqual(other.Methods.OrderBy(a => a)) &&
			   this.Name == other.Name &&
				this.AccessModifyer == other.AccessModifyer &&
			this.ImplementedInterfaces.OrderBy(a => a).SequenceEqual(other.ImplementedInterfaces.OrderBy(a => a)) &&
			this.Namespace == other.Namespace &&
			   this.Type.Equals(other.Type);

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = this.References?.GetHashCode() ?? 0;
				hashCode = (hashCode * 397) ^ (this.Usings?.GetHashCode() ?? 0);
				hashCode = (hashCode * 397) ^ (this.Methods?.GetHashCode() ?? 0);
				hashCode = (hashCode * 397) ^ (this.ImplementedInterfaces?.GetHashCode() ?? 0);
				hashCode = (hashCode * 397) ^ (this.OverrideGenericTypes?.GetHashCode() ?? 0);
				hashCode = (hashCode * 397) ^ (int)this.AccessModifyer;
				hashCode = (hashCode * 397) ^ (this.Type?.GetHashCode() ?? 0);
				return hashCode;
			}
		}
	}
}
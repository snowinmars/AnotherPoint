using AnotherPoint.Common;
using System.Collections.Generic;
using System.Linq;

namespace AnotherPoint.Entities
{
	public class Interface
	{
		public Interface(string fullTypeName)
		{
			this.Type = new MyType(fullTypeName);

			this.ImplementedInterfaces = new List<Interface>();

			this.Usings = new List<string>();
			this.Methods = new List<Method>();
		}

		public string Namespace { get; set; }

		public string FullName
		{
			get { return this.Type.FullName; }
			set { this.Type.FullName = value; }
		}

		public IList<string> Usings { get; }
		public IList<Method> Methods { get; }

		public string Name
		{
			get { return this.Type.Name; }
			set { this.Type.Name = value; }
		}

		public IList<Interface> ImplementedInterfaces { get; private set; }

		public AccessModifyer AccessModifyer { get; set; }

		public MyType Type { get; set; }

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
	}
}
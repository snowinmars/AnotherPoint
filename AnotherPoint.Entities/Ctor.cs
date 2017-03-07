using AnotherPoint.Common;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnotherPoint.Entities
{
	public class Ctor
	{
		public Ctor(string fullTypeName)
		{
			this.Type = new MyType(fullTypeName);
			this.AccessModifyer = AccessModifyer.Public;
			this.ArgumentCollection = new List<Argument>();

			this.IsCtorForInject = false;
		}

		public AccessModifyer AccessModifyer { get; set; }

		public IList<Argument> ArgumentCollection { get; }

		public bool IsCtorForInject { get; set; }

		public MyType Type { get; set; }

		public override bool Equals(object obj)
		{
			// ReSharper disable once ConditionIsAlwaysTrueOrFalse : it depends from compiler, google about callvirt and call CLR instructions
			if (this == null)
			{
				return false;
			}

			Ctor ctor = obj as Ctor;

			if (ctor == null)
			{
				return false;
			}

			return this.Equals(ctor);
		}

		public bool Equals(Ctor other)
			=> this.IsCtorForInject == other.IsCtorForInject &&
			   this.Type.Equals(other.Type) &&
			   this.AccessModifyer == other.AccessModifyer &&
			   this.ArgumentCollection.OrderBy(a => a).SequenceEqual(other.ArgumentCollection.OrderBy(a => a));

		public bool IsDefaultCtor(AccessModifyer? withAccessModifyer = null)
		{
			if (!withAccessModifyer.HasValue)
			{
				return this.ArgumentCollection.Count == 0;
			}

			return this.ArgumentCollection.Count == 0 && this.AccessModifyer.HasFlag(withAccessModifyer.Value);
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append($"{this.AccessModifyer.AsString()} {this.Type.Name} ({this.ArgumentCollection.Count} args)");

			return sb.ToString();
		}
	}
}
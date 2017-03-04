using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnotherPoint.Common;

namespace AnotherPoint.Entities
{
	public class Class
	{
		public Class(string fullTypeName)
		{
			Type = new MyType(fullTypeName);

			Ctors = new List<Ctor>();
			Fields = new List<Field>();
			Properties = new List<Property>();

			AccessModifyer = AccessModifyer.Public;
		}

		public string Namespace
		{
			get { return Type.Namespace; }
			set { Type.Namespace = value; }
		}

		public string Name
		{
			get { return Type.Name; }
			set { Type.Name = value; }
		}
		public MyType Type { get; set; }
		public AccessModifyer AccessModifyer { get; set; }
		public IList<Ctor> Ctors { get; private set; }
		public IList<Field> Fields { get; private set; }
		public IList<Property> Properties { get; private set; }
	}
}

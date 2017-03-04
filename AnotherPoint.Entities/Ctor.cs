using AnotherPoint.Common;
using System.Collections.Generic;

namespace AnotherPoint.Entities
{
	public class Ctor
	{
		public Ctor(string fullTypeName)
		{
			this.Type = new MyType(fullTypeName);
			this.ArgumentCollection = new List<CtorArgument>();
		}

		public AccessModifyer AccessModifyer { get; set; }
		public IList<CtorArgument> ArgumentCollection { get; private set; }
		public MyType Type { get; set; }
	}
}
﻿using AnotherPoint.Common;
using System.Collections.Generic;
using System.Text;

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
		public IList<CtorArgument> ArgumentCollection { get; }
		public MyType Type { get; set; }

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append($"{this.AccessModifyer.AsString()} {this.Type.Name} ({this.ArgumentCollection.Count} args)");

			return sb.ToString();
		}
	}
}
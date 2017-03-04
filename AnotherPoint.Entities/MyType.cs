using System;
using System.Collections.Generic;
using System.Linq;
using AnotherPoint.Common;

namespace AnotherPoint.Entities
{
	public class MyType
	{
		public MyType(string fullName)
		{
			GenericTypes = new List<string>();
			
				// If original type is generic, fullName here is like
			//   System.Collections.Generic.IEnumerable`1[[TmpConsoleApplication.User, TmpConsoleApplication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]

			// fullNameWithoutAssemblyInfo is like
			//  System.Collections.Generic.IEnumerable`1
			string f;
			string fullNameWithoutAssemblyInfo = fullName.Split(new[] { '[' }, StringSplitOptions.RemoveEmptyEntries).First();

			// if this type was generic, convert it to the human-readable form like
			//   System.Collections.Generic.IEnumerable
			FullName = Constant.binds.TryGetValue(fullNameWithoutAssemblyInfo, out f)
				? f 
				: fullName;

			Name = FullName.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries).Last();

			int v = FullName.LastIndexOf(".");

			if (v >= 0)
			{
				Namespace = FullName.Substring(0, v);
			}
		}

		public string Namespace { get; set; }
		public string Name { get; set; }
		public string FullName { get; set; }
		public bool IsGeneric { get; set; }
		public IList<string> GenericTypes { get; private set; }
	}
}
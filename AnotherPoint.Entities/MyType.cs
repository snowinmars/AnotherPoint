using AnotherPoint.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnotherPoint.Entities
{
	public class MyType
	{
		public MyType(string fullName)
		{
			this.GenericTypes = new List<string>();

			this.FullName = this.ParseFullName(fullName);
			this.Name = this.ParseName();
			this.Namespace = this.ParseNamespace();
			this.IsGeneric = null;
		}

		public string FullName { get; set; }

		public IList<string> GenericTypes { get; }

		public bool? IsGeneric { get; set; }

		public string Name { get; set; }

		public string Namespace { get; set; }

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append($"{this.Name}");

			if (this.IsGeneric.HasValue && this.IsGeneric.Value)
			{
				sb.Append($"<...> ({this.GenericTypes.Count})");
			}

			return sb.ToString();
		}

		private string ParseFullName(string fullName)
		{
			// If original type is generic, fullName here is like
			//   System.Collections.Generic.IEnumerable`1[[TmpConsoleApplication.User, TmpConsoleApplication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]

			// fullNameWithoutAssemblyInfo is like
			//  System.Collections.Generic.IEnumerable`1
			string f;
			string fullNameWithoutAssemblyInfo = fullName.Split(new[] { '[' }, StringSplitOptions.RemoveEmptyEntries).First();

			// if this type was generic, convert it to the human-readable form like
			//   System.Collections.Generic.IEnumerable
			return Constant.FullTypeNameHumanReadableBinding.TryGetValue(fullNameWithoutAssemblyInfo, out f) ? f : fullName;
		}

		private string ParseName()
		{
			return this.FullName.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries)
							.Last();
		}

		private string ParseNamespace()
		{
			int v = this.FullName.LastIndexOf(".", StringComparison.InvariantCultureIgnoreCase);

			return v >= 0 ? this.FullName.Substring(0, v) : this.FullName;
		}
	}
}
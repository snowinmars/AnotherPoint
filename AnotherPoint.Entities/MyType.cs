using AnotherPoint.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnotherPoint.Entities
{
	public class MyType : AnotherPointObject
	{
		public MyType(string fullName)
		{
			this.GenericTypes = new List<string>();

			this.FullName = this.ParseFullName(fullName);
			this.Name = this.ParseName();
			this.Namespace = this.ParseNamespace();
			this.IsGeneric = null;
			this.IsCollection = null;
		}

		public static MyType GenericMyType;

		static MyType()
		{
			MyType.GenericMyType = new MyType("T")
			{
				IsGeneric = true,
			};
		}

		public string FullName { get; set; }

		public IList<string> GenericTypes { get; }

		public bool? IsCollection { get; set; }

		public bool? IsGeneric { get; set; }

		public string Name { get; set; }

		public string Namespace { get; set; }

		public override bool Equals(object obj)
		{
			// ReSharper disable once ConditionIsAlwaysTrueOrFalse : it depends from compiler, google about callvirt and call CLR instructions
			if (this == null)
			{
				return false;
			}

			MyType myType = obj as MyType;

			if (myType == null)
			{
				return false;
			}

			return this.Equals(myType);
		}

		public bool Equals(MyType other)
			=> this.FullName == other.FullName &&
			   this.Name == other.Name &&
			   this.Namespace == other.Namespace &&
			   this.GenericTypes.OrderBy(a => a).SequenceEqual(other.GenericTypes.OrderBy(a => a)) &&
			   this.IsGeneric == other.IsGeneric &&
			   this.IsGeneric.HasValue == other.IsGeneric.HasValue &&
			   (!this.IsGeneric.HasValue || !other.IsGeneric.HasValue || this.IsGeneric.Value == other.IsGeneric.Value);

		public string GetFullNameWithoutAssemblyInfo(string fullName)
		{
			return fullName.Split(new[] { '[' }, StringSplitOptions.RemoveEmptyEntries).First();
		}

		// if there's values - they are equals
		public string GetFullTypeNameHumanReadable(string fullTypeNameWithoutAssemblyInfo)
		{
			return Constant.FullTypeNameHumanReadableBinding.TryGetValue(fullTypeNameWithoutAssemblyInfo, out string f) ?
				f :
				fullTypeNameWithoutAssemblyInfo;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = this.IsCollection.GetHashCode();
				hashCode = (hashCode * 397) ^ (this.FullName?.GetHashCode() ?? 0);
				hashCode = (hashCode * 397) ^ (this.GenericTypes?.GetHashCode() ?? 0);
				hashCode = (hashCode * 397) ^ this.IsGeneric.GetHashCode();
				hashCode = (hashCode * 397) ^ (this.Name?.GetHashCode() ?? 0);
				hashCode = (hashCode * 397) ^ (this.Namespace?.GetHashCode() ?? 0);
				return hashCode;
			}
		}

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
			string fullNameWithoutAssemblyInfo = this.GetFullNameWithoutAssemblyInfo(fullName);

			// if this type was generic, convert it to the human-readable form like
			//   System.Collections.Generic.IEnumerable
			return this.GetFullTypeNameHumanReadable(fullNameWithoutAssemblyInfo);
		}

		private string ParseName()
		{
			int index = this.FullName.IndexOf("<", StringComparison.InvariantCultureIgnoreCase);

			string n = index > 0 ? this.FullName.Substring(0, index) : this.FullName;

			return n.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries)
							.Last();
		}

		private string ParseNamespace()
		{
			int v = Helpers.NameWithoutGeneric(this.FullName).LastIndexOf(".", StringComparison.InvariantCultureIgnoreCase);

			return v >= 0 ? this.FullName.Substring(0, v) : this.FullName;
		}
	}
}
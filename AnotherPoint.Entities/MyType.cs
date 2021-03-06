﻿using AnotherPoint.Common;
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
			   (!this.IsGeneric.HasValue || !other.IsGeneric.HasValue || this.IsGeneric.Value == other.IsGeneric.Value); // if there's values - they are equals

		public string GetFullNameWithoutAssemblyInfo(string fullName)
		{
			return fullName.Split(new[] { '[' }, StringSplitOptions.RemoveEmptyEntries).First();
		}

		public string GetFullTypeNameHumanReadable(string fullTypeNameWithoutAssemblyInfo)
		{
			string f;

			return Constant.FullTypeNameHumanReadableBinding.TryGetValue(fullTypeNameWithoutAssemblyInfo, out f) ?
				f :
				fullTypeNameWithoutAssemblyInfo;
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
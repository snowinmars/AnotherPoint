﻿using AnotherPoint.Common;
using System.Collections.Generic;
using System.Text;

namespace AnotherPoint.Entities
{
	public class Class
	{
		public Class(string fullTypeName)
		{
			this.Type = new MyType(fullTypeName);

			this.Ctors = new List<Ctor>();
			this.Fields = new List<Field>();
			this.Usings = new List<string>();
			this.Properties = new List<Property>();
			this.Interfaces = new List<Interface>();
			this.Methods = new List<Method>();
			this.Constants = new List<Field>();

			this.AccessModifyer = AccessModifyer.Public;

			this.EntityPurposePair = new EntityPurposePair("", "");
			this.DestinationTypeName = "";
			this.IsEndpoint = false;
		}

		public IList<string> Usings { get; private set; }
		public AccessModifyer AccessModifyer { get; set; }
		public IList<Field> Constants { get; private set; }
		public IList<Ctor> Ctors { get; private set; }
		public string DestinationTypeName { get; set; }
		public IList<Field> Fields { get; private set; }
		public IList<Interface> Interfaces { get; }
		public bool IsEndpoint { get; set; }
		public IList<Method> Methods { get; }
		public EntityPurposePair EntityPurposePair { get; set; }

		public string Name
		{
			get
			{
				if (string.IsNullOrWhiteSpace(this.EntityPurposePair.Purpose) ||
				    string.IsNullOrWhiteSpace(this.EntityPurposePair.Entity))
				{
					return this.Type.Name;
				}

				return this.EntityPurposePair.Both;
			}
		}

		public string Namespace
		{
			get { return this.Type.Namespace; }
		}

		public IList<Property> Properties { get; private set; }
		public MyType Type { get; set; }

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(this.Type.Name);

			if (this.Type.IsGeneric.HasValue && this.Type.IsGeneric.Value)
			{
				sb.Append("<...>");
			}

			sb.Append($" {this.Name}");

			return sb.ToString();
		}
	}
}
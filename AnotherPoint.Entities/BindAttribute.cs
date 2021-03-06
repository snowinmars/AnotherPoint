﻿using AnotherPoint.Common;
using System;
using System.Text;

namespace AnotherPoint.Entities
{
	[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = true)]
	public class BindAttribute : Attribute
	{
		public BindAttribute(BindSettings settings, string name)
		{
			this.Settings = settings;
			this.Name = name;
		}

		public string Name { get; }
		public BindSettings Settings { get; }

		public override bool Equals(object obj)
		{
			// ReSharper disable once ConditionIsAlwaysTrueOrFalse : it depends from compiler, google about callvirt and call CLR instructions
			if (this == null)
			{
				return false;
			}

			BindAttribute bindAttribute = obj as BindAttribute;

			if (bindAttribute == null)
			{
				return false;
			}

			return this.Equals(bindAttribute);
		}

		public bool Equals(BindAttribute other)
			=> this.Name == other.Name &&
			   this.Settings == other.Settings;

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append($"{this.Name} to {this.Settings}");

			return sb.ToString();
		}
	}
}
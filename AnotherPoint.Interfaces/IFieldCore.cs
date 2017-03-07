using System;
using System.Reflection;
using AnotherPoint.Entities;

namespace AnotherPoint.Interfaces
{
	public interface IFieldCore : IDisposable
	{
		Field Map(FieldInfo fieldInfo);
		string RenderAccessModifyer(Field field);
		string RenderName(Field field);
		string RenderTypeName(Field field);
	}
}
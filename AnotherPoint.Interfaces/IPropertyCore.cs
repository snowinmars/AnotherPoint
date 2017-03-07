using System;
using System.Reflection;
using AnotherPoint.Entities;

namespace AnotherPoint.Interfaces
{
	public interface IPropertyCore : IDisposable
	{
		Property Map(PropertyInfo propertyInfo);
		string RenderGetMethodAccessModifyer(Property property);
		string RenderName(Property property);
		string RenderSetMethodAccessModifyer(Property property);
		string RenderTypeName(Property property);
	}
}
using AnotherPoint.Entities;
using System;
using System.Reflection;

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
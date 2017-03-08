using AnotherPoint.Entities;
using System;

namespace AnotherPoint.Interfaces
{
	public interface IInterfaceCore : IDisposable
	{
		Interface Map(Type interfaceType);
		string RenderAccessModifyer(Interface model);
		string RenderName(Interface model);
		string RenderNamespace(Interface model);
		string RenderCarrige(Interface @interface);
	}
}
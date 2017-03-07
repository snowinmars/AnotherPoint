using System;
using AnotherPoint.Entities;

namespace AnotherPoint.Interfaces
{
	public interface IClassCore : IDisposable
	{
		Class Map(Type type);
		string RenderAccessModifyer(Class @class);
		string RenderDefaultDestinationName(Class @class);
		string RenderInterfaces(Class @class);
		string RenderName(Class @class);
	}
}
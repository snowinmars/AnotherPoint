using AnotherPoint.Entities;
using System;

namespace AnotherPoint.Interfaces
{
	public interface IClassCore : IDisposable
	{
		Class Map(Type type);

		string RenderAccessModifyer(Class @class);

		string RenderDefaultDestinationName(Class @class);

		string RenderInterfaces(Class @class);

		string RenderName(Class @class);
		string RenderNamespace(Class @class);
		string RenderUsings(Class @class);
	}
}
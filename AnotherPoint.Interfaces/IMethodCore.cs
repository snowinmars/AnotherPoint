using System;
using System.Reflection;
using AnotherPoint.Entities;

namespace AnotherPoint.Interfaces
{
	public interface IMethodCore : IDisposable
	{
		Method Map(MethodInfo methodInfo, string className = null);
		string RenderAccessModifyer(Method method);
		string RenderArguments(Method method);
		string RenderBody(Method method);
		string RenderMethodName(Method method);
		string RenderReturnTypeName(Method method);
	}
}
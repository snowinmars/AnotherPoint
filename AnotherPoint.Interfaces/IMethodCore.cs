using AnotherPoint.Entities;
using System;
using System.Reflection;

namespace AnotherPoint.Interfaces
{
	public interface IMethodCore : IDisposable
	{
		Method Map(MethodInfo methodInfo, EntityPurposePair entityPurposePair);

		string RenderAccessModifyer(Method method);

		string RenderArguments(Method method);

		string RenderBody(Method method);

		string RenderMethodName(Method method);

		string RenderReturnTypeName(Method method);

		string RenderAdditionalBody(Method method);
	}
}
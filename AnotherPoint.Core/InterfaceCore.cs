using AnotherPoint.Common;
using AnotherPoint.Entities;
using System;
using AnotherPoint.Engine;
using AnotherPoint.Interfaces;

namespace AnotherPoint.Core
{
	public class InterfaceCore : IInterfaceCore
	{
		public Interface Map(Type interfaceType)
		{
			if (!interfaceType.IsInterface)
			{
				throw new ArgumentException($"Type {interfaceType.FullName} is not an interface");
			}

			Interface @interface = new Interface(interfaceType.FullName);

			HandleMethods(@interface, interfaceType);

			return @interface;
		}

		private void HandleMethods(Interface @interface, Type interfaceType)
		{
			foreach (var methodInfo in interfaceType.GetMethods(Constant.AllInstance))
			{
				Method method = RenderEngine.MethodCore.Map(methodInfo);

				@interface.Methods.Add(method);
			}
		}

		public void Dispose()
		{
		}
	}
}
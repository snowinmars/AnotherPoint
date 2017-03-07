using AnotherPoint.Common;
using AnotherPoint.Entities;
using System;
using System.Collections.Generic;
using System.Reflection;
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

			HandleMethods(interfaceType.GetMethods(Constant.AllInstance), @interface.Methods);

			return @interface;
		}

		private void HandleMethods(IEnumerable<MethodInfo> systemTypeMethods, ICollection<Method> interfaceMethods)
		{
			foreach (var methodInfo in systemTypeMethods)
			{
				Method method = RenderEngine.MethodCore.Map(methodInfo);

				interfaceMethods.Add(method);
			}
		}

		public void Dispose()
		{
		}
	}
}
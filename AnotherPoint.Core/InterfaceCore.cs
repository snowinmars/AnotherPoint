using AnotherPoint.Common;
using AnotherPoint.Engine;
using AnotherPoint.Entities;
using AnotherPoint.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace AnotherPoint.Core
{
	public class InterfaceCore : IInterfaceCore
	{
		public void Dispose()
		{
		}

		public Interface Map(Type interfaceType)
		{
			if (!interfaceType.IsInterface)
			{
				throw new ArgumentException($"Type {interfaceType.FullName} is not an interface");
			}

			Interface @interface = new Interface(interfaceType.FullName);

			this.HandleMethods(interfaceType.GetMethods(Constant.AllInstance), @interface.Methods);

			return @interface;
		}

		private void HandleMethods(IEnumerable<MethodInfo> systemTypeMethods, ICollection<Method> interfaceMethods)
		{
			foreach (var methodInfo in systemTypeMethods)
			{
				Method method = RenderEngine.MethodCore.Map(methodInfo, new EntityPurposePair("", ""));

				interfaceMethods.Add(method);
			}
		}
	}
}
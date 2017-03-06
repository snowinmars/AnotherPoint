using AnotherPoint.Common;
using AnotherPoint.Entities;
using System;

namespace AnotherPoint.Core
{
	public static class InterfaceCore
	{
		public static Interface Map(Type interfaceType)
		{
			if (!interfaceType.IsInterface)
			{
				throw new ArgumentException($"Type {interfaceType.FullName} is not an interface");
			}

			Interface @interface = new Interface(interfaceType.FullName);

			InterfaceCore.HandleMethods(@interface, interfaceType);

			return @interface;
		}

		private static void HandleMethods(Interface @interface, Type interfaceType)
		{
			foreach (var methodInfo in interfaceType.GetMethods(Constant.AllInstance))
			{
				Method method = MethodCore.Map(methodInfo);

				@interface.Methods.Add(method);
			}
		}
	}
}
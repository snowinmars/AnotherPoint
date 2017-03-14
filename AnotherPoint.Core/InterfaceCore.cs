using AnotherPoint.Common;
using AnotherPoint.Engine;
using AnotherPoint.Entities;
using AnotherPoint.Extensions;
using AnotherPoint.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

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

			Interface @interface = new Interface(interfaceType.FullName)
			{
				AccessModifyer = this.GetAccessModifyer(interfaceType),
				Namespace = interfaceType.Namespace
			};

			this.HandleMethods(interfaceType.GetMethods(Constant.AllInstance), @interface.Methods);
			this.HandleUsings(interfaceType.GetCustomAttributes<InsertUsingAttribute>(), @interface.Usings);

			foreach (var implementInterface in interfaceType.GetInterfaces())
			{
				@interface.ImplementedInterfaces.Add(this.Map(implementInterface));
			}

			return @interface;
		}

		public string RenderAccessModifyer(Interface model)
		{
			var accessModifyer = model.AccessModifyer;

			// Interfaces method have abstract and virtual access modifiers by default, but I mustn't render it.

			if (accessModifyer.HasFlag(AccessModifyer.Abstract))
			{
				accessModifyer -= AccessModifyer.Abstract;
			}

			if (accessModifyer.HasFlag(AccessModifyer.Virtual))
			{
				accessModifyer -= AccessModifyer.Virtual;
			}

			return accessModifyer.AsString();
		}

		public string RenderCarrige(Interface @interface)
		{
			if (@interface.ImplementedInterfaces.Count == 0)
			{
				return "";
			}

			StringBuilder sb = new StringBuilder();

			sb.Append(" : ");
			sb.Append(string.Join(",", @interface.ImplementedInterfaces.Select(i => i.FullName)));

			return sb.ToString();
		}

		public string RenderName(Interface model)
		{
			return model.Name;
		}

		public string RenderNamespace(Interface model)
		{
			return model.Namespace;
		}

		public string RenderUsings(Interface @interface)
		{
			StringBuilder sb = new StringBuilder();

			foreach (var interfaceUsing in @interface.Usings)
			{
				sb.AppendLine($"using {interfaceUsing};");
			}

			return sb.ToString();
		}

		private AccessModifyer GetAccessModifyer(Type interfaceType)
		{
			AccessModifyer accessModifyer = AccessModifyer.None;

			if (interfaceType.IsPublic)
			{
				accessModifyer |= AccessModifyer.Public;
			}

			if (interfaceType.IsInternal())
			{
				accessModifyer |= AccessModifyer.Internal;
			}

			return accessModifyer;
		}

		private void HandleMethods(IEnumerable<MethodInfo> systemTypeMethods, ICollection<Method> interfaceMethods)
		{
			foreach (var methodInfo in systemTypeMethods)
			{
				Method method = RenderEngine.MethodCore.Map(methodInfo, new EntityPurposePair("", ""));

				method.AccessModifyer |= AccessModifyer.Abstract | AccessModifyer.Virtual;

				interfaceMethods.Add(method);
			}
		}

		private void HandleUsings(IEnumerable<InsertUsingAttribute> getCustomAttributes, IList<string> interfaceUsings)
		{
			foreach (var insertUsingAttribute in getCustomAttributes)
			{
				interfaceUsings.Add(insertUsingAttribute.Using);
			}
		}
	}
}
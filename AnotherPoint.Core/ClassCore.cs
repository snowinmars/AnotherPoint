﻿using AnotherPoint.Common;
using AnotherPoint.Entities;
using AnotherPoint.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using AnotherPoint.Engine;
using AnotherPoint.Interfaces;

namespace AnotherPoint.Core
{
	public class ClassCore : IClassCore
	{
		public string RenderAccessModifyer(Class @class)
		{
			return @class.AccessModifyer.AsString();
		}

		public string RenderDefaultDestinationName(Class @class)
		{
			if (@class.IsEndpoint)
			{
				return $"{@class.Name}Destination";
			}

			throw new InvalidOperationException($"Class {@class.Name} is not an endpoint class, so it can't have destination field");
		}

		public string RenderInterfaces(Class @class)
		{
			if (@class.Interfaces.Count == 0)
			{
				return "";
			}

			StringBuilder sb = new StringBuilder();

			sb.Append(" : ");
			sb.AppendLine(string.Join(",", @class.Interfaces.Select(i => i.FullName)));

			return sb.ToString();
		}

		public string RenderName(Class @class)
		{
			return @class.Name;
		}

		public Class Map(Type type)
		{
			if (type.IsInterface)
			{
				throw new ArgumentException($"Type {type.FullName} is an interface, use InterfaceCore::Map for this type.");
			}

			Class @class = new Class(type.FullName)
			{
				AccessModifyer = GetAccessModifyer(type),
			};

			SetupEndpoint(type, @class);
			SetupGeneric(type, @class.Type);

			SetupFields(type.GetFields(Constant.AllInstance).Where(f => !f.IsAutogenerated()), @class);
			SetupProperties(type.GetProperties(Constant.AllInstance), @class.Properties);
			SetupCtors(type.GetConstructors(Constant.AllInstance), @class.Ctors);
			SetupInterfaces(type.GetInterfaces(), @class.Interfaces);
			SetupMethods(type.GetMethods(Constant.AllInstance | BindingFlags.DeclaredOnly)
								.Where(m => !m.Name.StartsWith(Constant.Get) && 
											!m.Name.StartsWith(Constant.Set)),
							@class.Methods,
							type.Name);

			return @class;
		}

		private AccessModifyer GetAccessModifyer(Type type)
		{
			AccessModifyer accessModifyer = AccessModifyer.None;

			if (type.IsPublic)
			{
				accessModifyer |= AccessModifyer.Public;
			}

			if (type.IsInternal())
			{
				accessModifyer |= AccessModifyer.Internal;
			}

			if (type.IsPrivate())
			{
				accessModifyer |= AccessModifyer.Private;
			}

			////////

			if (type.IsAbstract)
			{
				accessModifyer |= AccessModifyer.Abstract;
			}

			if (type.IsSealed)
			{
				accessModifyer |= AccessModifyer.Sealed;
			}

			return accessModifyer;
		}

		private Field GetDestinationFieldForInject(Class @class)
		{
			Field destinationField = new Field(this.RenderDefaultDestinationName(@class), @class.DestinationTypeName)
			{
				AccessModifyer = AccessModifyer.Private
			};

			destinationField.Name = destinationField.Name.FirstLetterToLower();

			return destinationField;
		}

		private Ctor GetInjectCtorForDestinationField(string typeFullName, Field destinationField)
		{
			Ctor injectedCtor = new Ctor(typeFullName)
			{
				AccessModifyer = AccessModifyer.Public,
				IsCtorForInject = true,
			};

			Argument arg = new Argument(destinationField.Name.FirstLetterToLower(),
											destinationField.Type.FullName,
											BindSettings.Exact);

			injectedCtor.ArgumentCollection.Add(arg);

			return injectedCtor;
		}

		private void SetupCtors(IEnumerable<ConstructorInfo> systemTypeCtors, ICollection<Ctor> classCtors)
		{
			foreach (var constructorInfo in systemTypeCtors)
			{
				classCtors.Add(RenderEngine.CtorCore.Map(constructorInfo));
			}

			// if there's only one explicit default ctor, there's no point to render it
			if (classCtors.Count == 1 &&
					classCtors.First().IsDefaultCtor())
			{
				classCtors.Clear();
			}

			// I dont want class have default ctor next to ctor for dependency inject, so if there's only two of them - I remove default one
			if (classCtors.Count == 2)
			{
				Ctor defaultCtor = classCtors.FirstOrDefault(c => c.IsDefaultCtor());
				Ctor injectCtor = classCtors.FirstOrDefault(c => c.IsCtorForInject);

				if (defaultCtor != null && injectCtor != null)
				{
					classCtors.Remove(defaultCtor);
				}
			}

			// the other cases user have to fix manually
		}

		private void SetupEndpoint(Type type, Class @class)
		{
			ClassImplAttribute classImplAttribute = type.GetCustomAttributes<ClassImplAttribute>()
																					.FirstOrDefault(attr => attr.IsEndPoint);

			if (classImplAttribute != null)
			{
				@class.IsEndpoint = true;
				@class.DestinationTypeName = classImplAttribute.DestinationTypeName;
			}
		}

		private void SetupFields(IEnumerable<FieldInfo> systemTypeFields, Class @class)
		{
			foreach (var fieldInfo in systemTypeFields)
			{
				@class.Fields.Add(RenderEngine.FieldCore.Map(fieldInfo));
			}

			if (!@class.IsEndpoint)
			{
				return;
			}

			// Class is endpoint when it have container to inject the dependency. So I have to create this field and the ctor for injection

			Field destinationField = GetDestinationFieldForInject(@class);

			@class.Fields.Add(destinationField);

			Ctor injectedCtor = GetInjectCtorForDestinationField(@class.Type.FullName, destinationField);

			@class.Ctors.Add(injectedCtor);
		}

		private void SetupGeneric(Type type, MyType myType)
		{
			myType.IsGeneric = type.IsGenericType;

			foreach (var genericTypeArgument in type.GenericTypeArguments)
			{
				myType.GenericTypes.Add(genericTypeArgument.FullName);
			}
		}

		private void SetupInterfaces(IEnumerable<Type> systemTypeInterfaces, ICollection<Interface> classInterfaces)
		{
			foreach (var interfaceType in systemTypeInterfaces)
			{
				Interface @interface = RenderEngine.InterfaceCore.Map(interfaceType);

				classInterfaces.Add(@interface);
			}
		}

		private void SetupMethods(IEnumerable<MethodInfo> systemTypeMethods, ICollection<Method> classMethods, string className)
		{
			foreach (var methodInfo in systemTypeMethods)
			{
				Method method = RenderEngine.MethodCore.Map(methodInfo, className);

				classMethods.Add(method);
			}
		}

		private void SetupProperties(IEnumerable<PropertyInfo> systemTypeProperties, ICollection<Property> classProperties)
		{
			foreach (var propertyInfo in systemTypeProperties)
			{
				classProperties.Add(RenderEngine.PropertyCore.Map(propertyInfo));
			}
		}

		public void Dispose()
		{
		}
	}
}
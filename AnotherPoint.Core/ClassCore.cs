﻿using AnotherPoint.Common;
using AnotherPoint.Engine;
using AnotherPoint.Entities;
using AnotherPoint.Extensions;
using AnotherPoint.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AnotherPoint.Core
{
	public class ClassCore : IClassCore
	{
		private readonly string[] reserverClassNamePostfixes;

		public ClassCore()
		{
			this.reserverClassNamePostfixes = new[] {
				"Logic",
				"Dao",
			};
		}

		public void Dispose()
		{
		}

		public Field GetDestinationFieldForInject(Class @class)
		{
			Field destinationField = new Field(this.RenderDefaultDestinationName(@class), @class.DestinationTypeName)
			{
				AccessModifyer = AccessModifyer.Private
			};

			destinationField.Name = destinationField.Name.FirstLetterToLower();

			return destinationField;
		}

		public Ctor GetInjectCtorForDestinationField(string typeFullName, Field destinationField)
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

		public Class Map(Type type)
		{
			if (type.IsInterface)
			{
				throw new ArgumentException($"Type {type.FullName} is an interface, use InterfaceCore::Map for this type.");
			}

			string logString = $"Mapping class {type.FullName}";

			Log.Info($"{logString}...");

			Stopwatch sw = Stopwatch.StartNew();

			Class @class = new Class(type.FullName)
			{
				AccessModifyer = this.GetAccessModifyer(type),
			};

			this.TryToResolveNameAndPurpose(type, @class);

			this.SetupEndpoint(type, @class);
			this.SetupGeneric(type, @class.Type);
			this.SetupCollection(type, @class.Type);

			this.SetupConstants(type.GetConstants(), @class.Constants);
			this.SetupNamespaces(type.GetCustomAttributes<InsertUsingAttribute>(), @class.Usings);
			this.SetupFields(type.GetFields(Constant.AllInstance).Where(f => !f.IsAutogenerated()), @class);
			this.SetupProperties(type.GetProperties(Constant.AllInstance), @class.Properties);
			this.SetupCtors(type.GetConstructors(Constant.AllInstance), @class.Ctors);
			this.SetupInterfaces(type.GetInterfaces(), @class.ImplementedInterfaces);
			this.SetupMethods(type, @class);
			this.SetupPackages(type, @class);

			Bag.TypePocket.AddOnce(type.GUID, type);
			Bag.ClassPocket.AddOnce(@class.Id, @class);
			Bag.MyTypePocket.AddOnce(@class.Type.Name, @class.Type);

			sw.Stop();

			Log.iDone(sw.Elapsed.TotalMilliseconds);

			return @class;
		}

		public string RenderAccessModifyer(Class @class)
		{
			return @class.AccessModifyer.AsString();
		}

		public string RenderDefaultDestinationName(Class @class)
		{
			if (@class.IsEndpoint)
			{
				return Helpers.GetDefaultDestinationName(@class.Name);
			}

			throw new InvalidOperationException($"Class {@class.Name} is not an endpoint class, so it can't have destination field");
		}

		public string RenderInterfaces(Class @class)
		{
			if (@class.ImplementedInterfaces.Count == 0)
			{
				return "";
			}

			StringBuilder sb = new StringBuilder();

			sb.Append(" : ");

			if (@class.OverrideGenericTypes.Count > 0)
			{
				string genericTypes = ClassCore.OverrideGenericTypes(@class);

				sb.Append(string.Join(",", @class.ImplementedInterfaces.Select(i => $"{i.Namespace}.{i.Name}{(i.Type.IsGeneric.IsTrue() ? $"<{genericTypes}>" : "")}")));
			}
			else
			{
				sb.AppendLine(string.Join(",", @class.ImplementedInterfaces.Select(i => i.FullName)));
			}

			return sb.ToString();
		}

		public string RenderName(Class @class)
		{
			return @class.Name;
		}

		public string RenderNamespace(Class @class)
		{
			return @class.Namespace;
		}

		public string RenderUsings(Class @class)
		{
			StringBuilder sb = new StringBuilder();

			foreach (var @using in @class.Usings)
			{
				sb.AppendLine($"using {@using};");
			}

			sb.AppendLine();

			return sb.ToString();
		}

		private static EntityPurposePair GetEntityPurposePair(Type type, Class @class)
		{
			string entity;
			string purpose;

			if (@class.EntityPurposePair.IsEmpty())
			{
				entity = type.Name;
				purpose = "";
			}
			else
			{
				entity = @class.EntityPurposePair.Entity;
				purpose = @class.EntityPurposePair.Purpose;
			}

			EntityPurposePair entityPurposePair = new EntityPurposePair(entity, purpose);
			return entityPurposePair;
		}

		private static string OverrideGenericTypes(Class @class)
		{
			StringBuilder s = new StringBuilder();

			foreach (var implementedInterface in @class.ImplementedInterfaces)
			{
				foreach (var genericType in implementedInterface.Type.GenericTypes)
				{
					if (@class.OverrideGenericTypes.ContainsKey(genericType))
					{
						s.Append($" {@class.OverrideGenericTypes[genericType]} ");
					}
					else
					{
						s.Append($" {genericType} ");
					}
				}
			}

			return s.ToString();
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

			if (type.IsStatic())
			{
				accessModifyer |= AccessModifyer.Static;
			}

			return accessModifyer;
		}

		private void SetupCollection(Type type, MyType classType)
		{
			classType.IsCollection = type.GetInterface(Constant.IEnumerable) != null;
		}

		private void SetupConstants(IEnumerable<FieldInfo> getConstants, IList<Field> classConstants)
		{
			foreach (var constant in getConstants)
			{
				classConstants.Add(RenderEngine.FieldCore.Map(constant));
			}
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

			Field destinationField = this.GetDestinationFieldForInject(@class);

			@class.Fields.Add(destinationField);

			Ctor injectedCtor = this.GetInjectCtorForDestinationField(@class.Type.FullName, destinationField);

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

		private void SetupMethods(Type type, Class @class)
		{
			EntityPurposePair entityPurposePair = ClassCore.GetEntityPurposePair(type, @class);

			this.SetupMethodsImplementation(type.GetMethods(Constant.AllInstance | BindingFlags.DeclaredOnly)
								.Where(m => !m.Name.StartsWith(Constant.Get) &&
											!m.Name.StartsWith(Constant.Set)),
							@class.Methods,
							entityPurposePair);
		}

		private void SetupMethodsImplementation(IEnumerable<MethodInfo> systemTypeMethods, ICollection<Method> classMethods, EntityPurposePair entityPurposePair)
		{
			foreach (var methodInfo in systemTypeMethods)
			{
				Method method = RenderEngine.MethodCore.Map(methodInfo, entityPurposePair);

				classMethods.Add(method);
			}
		}

		private void SetupNamespaces(IEnumerable<InsertUsingAttribute> insertUsingAttributes, ICollection<string> usings)
		{
			foreach (var insertUsingAttribute in insertUsingAttributes)
			{
				usings.Add(insertUsingAttribute.Using);
			}
		}

		private void SetupPackages(Type type, Class @class)
		{
			foreach (var insertNugetPackageAttribute in type.GetCustomAttributes<InsertNugetPackageAttribute>())
			{
				@class.PackageAttributes.Add(insertNugetPackageAttribute);
			}
		}

		private void SetupProperties(IEnumerable<PropertyInfo> systemTypeProperties, ICollection<Property> classProperties)
		{
			foreach (var propertyInfo in systemTypeProperties)
			{
				classProperties.Add(RenderEngine.PropertyCore.Map(propertyInfo));
			}
		}

		private void TryToResolveNameAndPurpose(Type type, Class @class)
		{
			foreach (var item in this.reserverClassNamePostfixes)
			{
				if (type.Name.EndsWith(item))
				{
					@class.EntityPurposePair.Entity = @class.Name.Remove(@class.Name.Length - item.Length, item.Length);
					@class.EntityPurposePair.Purpose = item;
				}
			}
		}
	}
}
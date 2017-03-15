using AnotherPoint.Common;
using AnotherPoint.Engine;
using AnotherPoint.Entities;
using AnotherPoint.Extensions;
using AnotherPoint.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AnotherPoint.Core
{
	public class ValidationCore : IValidationCore
	{
		private readonly string[] dataAnnotationAttributesFullName =
		{
			"System.ComponentModel.DataAnnotations.DataTypeAttribute",
			"System.ComponentModel.DataAnnotations.RangeAttribute",
			"System.ComponentModel.DataAnnotations.RegularExpressionAttribute",
			"System.ComponentModel.DataAnnotations.StringLengthAttribute",
		};

		private readonly IDictionary<Property, ICollection<Attribute>> propertyAttributeBinding;

		public ValidationCore()
		{
			this.propertyAttributeBinding = new Dictionary<Property, ICollection<Attribute>>();
		}

		public static Type FindType(string qualifiedTypeName)
		{
			Type t = Type.GetType(qualifiedTypeName);

			if (t != null)
			{
				return t;
			}

			foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
			{
				//Debugger.Launch();

				t = asm.GetType(qualifiedTypeName);

				if (t != null)
				{
					return t;
				}
			}

			return null;
		}

		public Class ConstructValidationClass(string @namespace)
		{
			Log.Info($"Constructing validation class for {@namespace}...");

			Stopwatch sw = Stopwatch.StartNew();

			Class @class = new Class(Constant.Validation)
			{
				Type = { Namespace = @namespace },
				AccessModifyer = AccessModifyer.Internal | AccessModifyer.Sealed
			};

			ValidationCore.SetupUsings(@class);
			ValidationCore.SetupReferences(@class);

			foreach (var type in Bag.TypePocket.Values)
			{
				PropertyInfo[] props = type.GetProperties();

				if (!props.Any())
				{
					continue;
				}

				Method checkMethod = new Method("Check", Constant.Types.SystemVoid)
				{
					AccessModifyer = AccessModifyer.Public | AccessModifyer.Static
				};

				Argument argument = this.ParseArgumentToCollection(props, checkMethod.Arguments);
				checkMethod.AdditionalBody = this.GetCheckMethodBody(argument);

				if (!@class.Methods.Contains(checkMethod))
				{
					@class.Methods.Add(checkMethod);
				}
			}

			sw.Stop();

			Log.iDone(sw.Elapsed.TotalMilliseconds);

			return @class;
		}

		private static void SetupReferences(Class @class)
		{
			@class.References.Add(Constant.Usings.System);
		}

		private static void SetupUsings(Class @class)
		{
			@class.Usings.Add(Constant.Usings.System);
			@class.Usings.Add(Constant.Usings.SystemTextRegularExpressions);
		}

		private string GetCheckMethodBody(Argument argument)
		{
			StringBuilder body = new StringBuilder();

			foreach (var propertyAttributePair in this.propertyAttributeBinding.Where(pair => pair.Value.Any()))
			{
				var property = propertyAttributePair.Key;
				var attributes = propertyAttributePair.Value;

				foreach (var attribute in attributes)
				{
					this.RenderAttribute(attribute, argument, property, body);
				}
			}

			return body.ToString();
		}

		private Argument ParseArgumentToCollection(IEnumerable<PropertyInfo> props, ICollection<Argument> arguments)
		{
			Argument argument = null;

			foreach (var propertyInfo in props)
			{
				this.SetupDataAnnotationsAttributes(propertyInfo);

				if (arguments.Count == 0)
				{
					Type declaringType = propertyInfo.DeclaringType;

					argument = new Argument(declaringType.Name.FirstLetterToLower(), declaringType.FullName, BindSettings.Validate);

					arguments.Add(argument);
				}
			}

			if (argument == null)
			{
				throw new InvalidOperationException("Why argument is null, yo, bro?");
			}

			return argument;
		}

		private void RenderAttribute(Attribute attribute, Argument argument, Property property, StringBuilder body)
		{
			var rangeAttribute = attribute as RangeAttribute;
			var regularExpressionAttribute = attribute as RegularExpressionAttribute;

			if (rangeAttribute != null)
			{
				this.RenderRangeAttribute(argument, property, body, rangeAttribute);
			}

			body.AppendLine();

			if (regularExpressionAttribute != null)
			{
				this.RenderRegularExpressionAttribute(argument, property, body, regularExpressionAttribute);
			}
		}

		private void RenderRangeAttribute(Argument argument, Property property, StringBuilder body, RangeAttribute rangeAttribute)
		{
			body.Append($"if (!({argument.Name}.{property.Name} > {rangeAttribute.Minimum} && ");
			body.AppendLine();
			body.Append($"{argument.Name}.{property.Name} < {rangeAttribute.Maximum} ))");
			body.AppendLine();
			body.AppendLine("{");
			body.AppendLine("throw new Exception();");
			body.AppendLine("}");
		}

		private void RenderRegularExpressionAttribute(Argument argument, Property property, StringBuilder body, RegularExpressionAttribute regularExpressionAttribute)
		{
			body.Append($"if (!(Regex.Match({argument.Name}.{property.Name}, \"{regularExpressionAttribute.Pattern}\").Success))");
			body.AppendLine();
			body.AppendLine("{");
			body.AppendLine("throw new Exception();");
			body.AppendLine("}");
		}

		private void SetupDataAnnotationsAttributes(PropertyInfo prop)
		{
			var property = RenderEngine.PropertyCore.Map(prop);
			this.propertyAttributeBinding.Add(property, new List<Attribute>());

			foreach (var dataAnnotationAttributeFullName in this.dataAnnotationAttributesFullName)
			{
				string d = "NOPE";

				try
				{
					d = dataAnnotationAttributeFullName;

					foreach (var customAttribute in prop.GetCustomAttributes(ValidationCore.FindType(dataAnnotationAttributeFullName)))
					{
						this.propertyAttributeBinding[property].Add(customAttribute);
					}
				}
				catch (Exception e)
				{
					Console.WriteLine(d);
					throw;
				}
			}
		}
	}
}
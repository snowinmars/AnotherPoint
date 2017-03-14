using AnotherPoint.Common;
using AnotherPoint.Engine;
using AnotherPoint.Entities;
using AnotherPoint.Extensions;
using AnotherPoint.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace AnotherPoint.Core
{
	public class ValidationCore : IValidationCore
	{
		public Class ConstructValidationClass(string @namespace)
		{
			Class @class = new Class("Validator");
			@class.Type.Namespace = @namespace;
			@class.AccessModifyer = AccessModifyer.Internal | AccessModifyer.Sealed;
			@class.Usings.Add(Constant.Usings.System);
			@class.Usings.Add(Constant.Usings.System_Text_RegularExpressions);

			@class.References.Add(Constant.Usings.System);

			foreach (var type in Bag.TypePocket.Values)
			{
				var props = type.GetProperties();

				if (props.Length > 0)
				{
					Method m = new Method("Check", "System.Void");
					m.AccessModifyer = AccessModifyer.Public | AccessModifyer.Static;
					Argument argument = null;

					foreach (var propertyInfo in props)
					{
						InitDataAnnotationsAttributes(propertyInfo);

						if (m.Arguments.Count == 0)
						{
							Type declaringType = propertyInfo.DeclaringType;
							argument = new Argument(declaringType.Name.FirstLetterToLower(), declaringType.FullName, BindSettings.Validate);
							m.Arguments.Add(argument);
						}
					}

					StringBuilder body = new StringBuilder();

					foreach (var pair in this.propertyAttributeBinding)
					{
						if (pair.Value.Count > 0)
						{
							foreach (var attribute in pair.Value)
							{
								var rangeAttribute = attribute as RangeAttribute;
								var regularExpressionAttribute = attribute as RegularExpressionAttribute;

								if (rangeAttribute != null)
								{
									body.Append($"if (!({argument.Name}.{pair.Key.Name} > {rangeAttribute.Minimum} && ");
									body.AppendLine();
									body.Append($"{argument.Name}.{pair.Key.Name} < {rangeAttribute.Maximum} ))");
									body.AppendLine();
									body.AppendLine("{");
									body.AppendLine("throw new Exception();");
									body.AppendLine("}");
								}

								body.AppendLine();

								if (regularExpressionAttribute != null)
								{
									body.Append($"if (!(Regex.Match({argument.Name}.{pair.Key.Name}, \"{regularExpressionAttribute.Pattern}\").Success))");
									body.AppendLine();
									body.AppendLine("{");
									body.AppendLine("throw new Exception();");
									body.AppendLine("}");
								}
							}
						}
					}

					m.AdditionalBody = body.ToString();

					if (!@class.Methods.Contains(m))
					{
						@class.Methods.Add(m);
					}
				}
			}

			return @class;
		}

		private string[] a =
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

		private void InitDataAnnotationsAttributes(PropertyInfo prop)
		{
			var property = RenderEngine.PropertyCore.Map(prop);
			propertyAttributeBinding.Add(property, new List<Attribute>());

			foreach (var dataAnnotationAttributeName in a)
			{
				foreach (var customAttribute in prop.GetCustomAttributes(ValidationCore.FindType(dataAnnotationAttributeName)))
				{
					this.propertyAttributeBinding[property].Add(customAttribute);
				}
			}
		}

		public static Type FindType(string qualifiedTypeName)
		{
			Type t = Type.GetType(qualifiedTypeName);

			if (t != null)
			{
				return t;
			}
			else
			{
				foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
				{
					t = asm.GetType(qualifiedTypeName);
					if (t != null)
						return t;
				}
				return null;
			}
		}
	}
}
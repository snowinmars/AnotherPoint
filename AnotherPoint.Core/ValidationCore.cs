using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AnotherPoint.Common;
using AnotherPoint.Engine;
using AnotherPoint.Entities;
using AnotherPoint.Entities.MethodImpl;
using AnotherPoint.Extensions;
using AnotherPoint.Interfaces;

namespace AnotherPoint.Core
{
	public class ValidationCore : IValidationCore
	{
		public Class ConstructValidationClass(string @namespace, string forDestination)
		{
			Class @class = new Class("Validator");
			@class.Type.Namespace = @namespace;
			@class.AccessModifyer = AccessModifyer.Internal | AccessModifyer.Sealed;

			foreach (var type in Bag.TypePocket.Values)
			{
				foreach (var propertyInfo in type.GetProperties())
				{
					IDictionary<string, ICollection<Attribute>> dataAnnotationsAttributes = new Dictionary<string, ICollection<Attribute>>();

					foreach (var attribute in this.GetDataAnnotationsAttributes(propertyInfo))
					{
						dataAnnotationsAttributes.Add(attribute);
					}

					Type declaringType = propertyInfo.DeclaringType;

					Method m = new Method("Check", "System.Void");

					m.Arguments.Add(new Argument(declaringType.Name.FirstLetterToLower(), declaringType.FullName, BindSettings.Validate));

					m.AccessModifyer = AccessModifyer.Public;

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

		private IDictionary<string, ICollection<Attribute>> GetDataAnnotationsAttributes(PropertyInfo prop)
		{
			IDictionary<string, ICollection<Attribute>> result = new Dictionary<string, ICollection<Attribute>>();

			foreach (var dataAnnotationAttributeName in this.a)
			{
				result.Add(dataAnnotationAttributeName, new List<Attribute>());
			}

			foreach (var pair in result)
			{
				foreach (var customAttribute in prop.GetCustomAttributes(ValidationCore.FindType(pair.Key)))
				{
					pair.Value.Add(customAttribute);
				}
			}

			return result;
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

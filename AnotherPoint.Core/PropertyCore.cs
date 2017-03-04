using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AnotherPoint.Common;
using AnotherPoint.Entities;

namespace AnotherPoint.Core
{
	public class PropertyCore
	{
		public Property Map (PropertyInfo propertyInfo)
		{
			string propertyName = propertyInfo.Name;
			string propertyType = Helpers.GetCorrectCollectionTypeNaming(propertyInfo.PropertyType.Name);

			Property property = new Property(propertyName, propertyType);
			
			// saving field name and type for further appeals from ctor

			Bag.Pocket[propertyName] = propertyType;

			return property;
		}
	}
}

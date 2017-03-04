using AnotherPoint.Entities;
using System.Reflection;

namespace AnotherPoint.Core
{
	public class PropertyCore
	{
		public Property Map(PropertyInfo propertyInfo)
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
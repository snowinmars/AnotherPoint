using AnotherPoint.Common;
using System;
using Xunit;

namespace AnotherPoint.Entities.Tests
{
	public class PropertyTests
	{
		[Fact]
		public void Ctor()
		{
			string propertyName = Guid.NewGuid().ToString();
			string typeName = Guid.NewGuid().ToString();

			Property p = new Property(propertyName, typeName);

			Assert.NotNull(p.Type);
			Assert.NotNull(p.GetMethod);
			Assert.NotNull(p.SetMethod);
			Assert.Equal(expected: propertyName, actual: p.Name);
			Assert.Equal(expected: typeName, actual: p.Type.Name);
			Assert.NotEqual(expected: AccessModifyer.None, actual: p.AccessModifyer);
		}
	}
}
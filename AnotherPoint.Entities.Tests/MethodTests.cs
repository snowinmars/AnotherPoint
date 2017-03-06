using AnotherPoint.Common;
using System;
using Xunit;

namespace AnotherPoint.Entities.Tests
{
	public class MethodTests
	{
		[Fact]
		public void Ctor()
		{
			string methodName = Guid.NewGuid().ToString();
			string typeName = Guid.NewGuid().ToString();

			Method m = new Method(methodName, typeName);

			Assert.Equal(expected: methodName, actual: m.Name);
			Assert.NotEqual(expected: AccessModifyer.None, actual: m.AccessModifyer);
		}
	}
}
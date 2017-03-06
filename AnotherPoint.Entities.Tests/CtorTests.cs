using AnotherPoint.Common;
using System;
using Xunit;

namespace AnotherPoint.Entities.Tests
{
	public class CtorTests
	{
		[Fact]
		public void Ctor()
		{
			string ctorName = Guid.NewGuid().ToString();

			Ctor ctor = new Ctor(ctorName);

			Assert.NotNull(ctor.Type);
			Assert.NotEqual(expected: AccessModifyer.None, actual: ctor.AccessModifyer);
			Assert.NotNull(ctor.ArgumentCollection);
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnotherPoint.Common;
using Xunit;

namespace AnotherPoint.Entities.Tests
{
	public class MethodTests
	{

		[Fact]
		public void Ctor()
		{
			string methodName = Guid.NewGuid().ToString();

			Method m = new Method(methodName);

			Assert.Equal(expected: methodName, actual: m.Name);
			Assert.NotEqual(expected: AccessModifyer.None, actual: m.AccessModifyer);
		}
	}
}

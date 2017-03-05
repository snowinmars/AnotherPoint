using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnotherPoint.Common;
using Xunit;

namespace AnotherPoint.Entities.Tests
{
	public class ClassTests
	{
		[Fact]
		public void Ctor()
		{
			string typeName = Guid.NewGuid().ToString();

			Class c = new Class(typeName);
			
			Assert.NotNull(c.Type);
			Assert.NotNull(c.Ctors);
			Assert.NotNull(c.Fields);
			Assert.NotNull(c.Properties);
			Assert.False(string.IsNullOrWhiteSpace(c.Namespace));
			Assert.Equal(expected: typeName, actual: c.Type.Name);
			Assert.NotEqual(expected: AccessModifyer.None, actual: c.AccessModifyer);
		}
	}
}

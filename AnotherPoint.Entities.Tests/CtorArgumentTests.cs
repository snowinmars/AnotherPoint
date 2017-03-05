using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnotherPoint.Common;
using Xunit;

namespace AnotherPoint.Entities.Tests
{
	public class CtorArgumentTests
	{
		[Fact]
		public void Ctor()
		{
			string argName = Guid.NewGuid().ToString();
			string typeName = Guid.NewGuid().ToString();
			const BindSettings bind = BindSettings.New;

			Argument arg = new Argument(argName, typeName, bind);
			
			Assert.NotNull(arg.Type);
			Assert.Equal(expected: bind, actual: arg.BindAttribute);
			Assert.Equal(expected: argName, actual: arg.Name);
			Assert.Equal(expected: typeName, actual: arg.Type.Name);
		}
	}
}

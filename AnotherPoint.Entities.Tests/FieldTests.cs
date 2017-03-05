using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnotherPoint.Common;
using Xunit;

namespace AnotherPoint.Entities.Tests
{
    public class FieldTests
    {
		[Fact]
	    public void Ctor()
	    {
		    string fieldName = Guid.NewGuid().ToString();
			string typeName = Guid.NewGuid().ToString();

			Field f = new Field(fieldName, typeName);
			
			Assert.NotNull(f.Type);
			Assert.Equal(expected: fieldName, actual: f.Name);
			Assert.Equal(expected: typeName, actual: f.Type.Name);
			Assert.NotEqual(expected: AccessModifyer.None, actual:f.AccessModifyer);
		}
	}
}

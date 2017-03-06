using Xunit;

namespace AnotherPoint.Entities.Tests
{
	public class MyTypeTests
	{
		[Fact]
		public void Ctor()
		{
			const string fullTypeName = "System.Int32";
			const string typeName = "Int32";
			const string @namespace = "System";

			MyType myType = new MyType(fullTypeName);

			Assert.Null(myType.IsGeneric);
			Assert.NotNull(myType.GenericTypes);
			Assert.Equal(expected: fullTypeName, actual: myType.FullName);
			Assert.Equal(expected: typeName, actual: myType.Name);
			Assert.Equal(expected: @namespace, actual: myType.Namespace);
		}
	}
}
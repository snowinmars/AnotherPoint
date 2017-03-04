using AnotherPoint.Common;

namespace AnotherPoint.Entities
{
	public class Method
	{
		public Method(string name)
		{
			Name = name;
			AccessModifyer = AccessModifyer.Public;
		}

		public string Name { get; set; }
		public AccessModifyer AccessModifyer { get; set; }
	}
}
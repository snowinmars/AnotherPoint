using AnotherPoint.Common;

namespace AnotherPoint.Entities
{
	public class Method
	{
		public Method(string name)
		{
			this.Name = name;
			this.AccessModifyer = AccessModifyer.Public;
		}

		public AccessModifyer AccessModifyer { get; set; }
		public string Name { get; set; }
	}
}
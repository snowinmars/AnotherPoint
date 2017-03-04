using AnotherPoint.Common;
using System.Text;

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

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append($"{this.Name}");

			return sb.ToString();
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnotherPoint.Core
{
	public static class Bag
	{
		static Bag()
		{
			Pocket = new Dictionary<string, string>();
		}

		public static IDictionary<string, string> Pocket { get; private set; }
	}
}

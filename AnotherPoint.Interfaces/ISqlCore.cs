using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnotherPoint.Entities;

namespace AnotherPoint.Interfaces
{
	public interface ISqlCore
	{
		void ConstructSqlScripts(IEnumerable<Endpoint> endpoints, string fullPathToDir);
	}
}

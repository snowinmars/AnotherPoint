using AnotherPoint.Entities;
using System.Collections.Generic;

namespace AnotherPoint.Interfaces
{
	public interface ISqlCore
	{
		void ConstructSqlScripts(IEnumerable<Endpoint> endpoints, string fullPathToDir);
	}
}
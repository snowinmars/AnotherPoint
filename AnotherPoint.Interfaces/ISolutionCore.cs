using System.Collections.Generic;
using AnotherPoint.Entities;

namespace AnotherPoint.Interfaces
{
	public interface ISolutionCore
	{
		void ConstructSolution(IEnumerable<Endpoint> endpoints, string fullPathToDir);
	}
}
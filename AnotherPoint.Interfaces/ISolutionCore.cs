using AnotherPoint.Entities;
using System.Collections.Generic;

namespace AnotherPoint.Interfaces
{
	public interface ISolutionCore
	{
		void ConstructSolution(IEnumerable<Endpoint> endpoints, string fullPathToDir);
	}
}
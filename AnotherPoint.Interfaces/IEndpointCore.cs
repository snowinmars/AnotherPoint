using AnotherPoint.Entities;

namespace AnotherPoint.Interfaces
{
	public interface IEndpointCore
	{
		string AppName { get; }

		Endpoint ConstructEndpointFor(Class entityClass);
	}
}
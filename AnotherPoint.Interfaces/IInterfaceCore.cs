using System;
using AnotherPoint.Entities;

namespace AnotherPoint.Interfaces
{
	public interface IInterfaceCore : IDisposable
	{
		Interface Map(Type interfaceType);
	}
}
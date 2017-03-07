using AnotherPoint.Entities;
using System;

namespace AnotherPoint.Interfaces
{
	public interface IInterfaceCore : IDisposable
	{
		Interface Map(Type interfaceType);
	}
}
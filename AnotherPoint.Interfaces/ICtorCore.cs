﻿using AnotherPoint.Entities;
using System;
using System.Reflection;

namespace AnotherPoint.Interfaces
{
	public interface ICtorCore : IDisposable
	{
		Ctor Map(ConstructorInfo constructorInfo);

		string RenderAccessModifyer(Ctor ctor);

		string RenderArgumentCollection(Ctor ctor);

		string RenderBody(Ctor ctor);

		string RenderCtorCarriage(Ctor ctor);

		string RenderTypeName(Ctor ctor);
	}
}
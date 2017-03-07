using System;

namespace AnotherPoint.Common
{
	[Flags]
	public enum AccessModifyer
	{
		None = 0,
		Public = 1,
		Internal = 2,
		Protected = 4,
		Private = 8,
		Abstract = 16,
		Sealed = 32,
		Virtual = 64,
		Static = 128,
	}
}
using System;
using System.Text;
using AnotherPoint.Common;

namespace AnotherPoint.Entities
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class MethodImpl_ShutMeUpAttribute : Attribute
	{
	}
}

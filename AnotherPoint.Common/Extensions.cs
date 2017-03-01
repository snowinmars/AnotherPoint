﻿using System;
using System.Reflection;

namespace AnotherPoint.Common
{
	public static class Extensions
	{
		public static bool IsAutogenerated(this FieldInfo fieldInfo)
			=> fieldInfo.Name.Contains(Constant.BackgroundEntityMark);

		#region FieldInfo

		public static bool IsInternal(this FieldInfo fieldInfo)
			=> fieldInfo.IsAssembly;

		public static bool IsProtected(this FieldInfo fieldInfo)
			=> fieldInfo.IsFamily;

		public static bool IsProtectedInternal(this FieldInfo fieldInfo)
			=> fieldInfo.IsFamilyOrAssembly;

		#endregion FieldInfo

		#region MethodInfo

		public static bool IsInternal(this MethodInfo methodInfo)
			=> methodInfo.IsAssembly;

		public static bool IsProtected(this MethodInfo methodInfo)
			=> methodInfo.IsFamily;

		public static bool IsProtectedInternal(this MethodInfo methodInfo)
			=> methodInfo.IsFamilyOrAssembly;

		#endregion MethodInfo

		#region Type

		public static bool IsInternal(this Type type)
			=> !type.IsPublic && !type.IsPrivate();

		public static bool IsPrivate(this Type type)
					=> type.IsNotPublic && type.IsNested;

		#endregion Type
	}
}
﻿using AnotherPoint.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AnotherPoint.Extensions
{
	public static class Extensions
	{
		public static void Clear(this DirectoryInfo directory)
		{
			foreach (FileInfo file in directory.GetFiles())
			{
				file.Delete();
			}

			foreach (System.IO.DirectoryInfo subDirectory in directory.GetDirectories())
			{
				subDirectory.Delete(true);
			}
		}

		public static string FirstLetterToLower(this string str)
		{
			if (string.IsNullOrWhiteSpace(str))
			{
				return str;
			}

			if (!char.IsLetter(str[0]))
			{
				return str;
			}

			StringBuilder sb = new StringBuilder(str.Length);
			sb.Append(str);
			sb[0] = char.ToLower(sb[0], CultureInfo.CurrentCulture);

			return sb.ToString();
		}

		public static string FirstLetterToUpper(this string str)
		{
			if (string.IsNullOrWhiteSpace(str))
			{
				return str;
			}

			if (!char.IsLetter(str[0]))
			{
				return str;
			}

			StringBuilder sb = new StringBuilder(str.Length);
			sb.Append(str);
			sb[0] = char.ToUpper(sb[0], CultureInfo.CurrentCulture);

			return sb.ToString();
		}

		public static bool IsAutogenerated(this FieldInfo fieldInfo)
							=> fieldInfo.Name.Contains(Constant.BackgroundEntityMark);

		public static StringBuilder RemoveLastSymbol(this StringBuilder sb)
		{
			return sb.Remove(sb.Length - 1, 1);
		}

		#region ConstructorInfo

		public static bool IsInternal(this ConstructorInfo constructorInfo)
			=> constructorInfo.IsAssembly;

		public static bool IsProtected(this ConstructorInfo constructorInfo)
			=> constructorInfo.IsFamily;

		public static bool IsProtectedInternal(this ConstructorInfo constructorInfo)
			=> constructorInfo.IsFamilyOrAssembly;

		#endregion ConstructorInfo

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

		public static IEnumerable<FieldInfo> GetConstants(this Type type)
		{
			return type.GetFields(BindingFlags.Public |
								  BindingFlags.Static |
								  BindingFlags.FlattenHierarchy)
				.Where(fieldInfo => fieldInfo.IsLiteral &&
									!fieldInfo.IsInitOnly)
				.ToList();
		}

		public static bool IsInternal(this Type type)
					=> !type.IsPublic && !type.IsPrivate();

		public static bool IsPrivate(this Type type)
			=> type.IsNotPublic && type.IsNested;

		public static bool IsStatic(this Type type)
			=> type.IsClass &&
			   type.IsSealed &&
			   type.IsAbstract;

		#endregion Type
	}
}
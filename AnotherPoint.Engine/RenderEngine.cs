﻿using AnotherPoint.Interfaces;

namespace AnotherPoint.Engine
{
	public static class RenderEngine
	{
		public static void Init(IClassCore classCore,
			ICtorCore ctorCore,
			IFieldCore fieldCore,
			IInterfaceCore interfaceCore,
			IMethodCore methodCore,
			IPropertyCore propertyCore)
		{
			RenderEngine.ClassCore = classCore;
			RenderEngine.CtorCore = ctorCore;
			RenderEngine.FieldCore = fieldCore;
			RenderEngine.InterfaceCore = interfaceCore;
			RenderEngine.MethodCore = methodCore;
			RenderEngine.PropertyCore = propertyCore;
		}

		public static IClassCore ClassCore { get; private set; }
		public static ICtorCore CtorCore { get; private set; }
		public static IFieldCore FieldCore { get; private set; }
		public static IInterfaceCore InterfaceCore { get; private set; }
		public static IMethodCore MethodCore { get; private set; }
		public static IPropertyCore PropertyCore { get; private set; }

		public static void Dispose()
		{
			RenderEngine.ClassCore.Dispose();
			RenderEngine.CtorCore.Dispose();
			RenderEngine.FieldCore.Dispose();
			RenderEngine.InterfaceCore.Dispose();
			RenderEngine.MethodCore.Dispose();
			RenderEngine.PropertyCore.Dispose();
		}
	}
}
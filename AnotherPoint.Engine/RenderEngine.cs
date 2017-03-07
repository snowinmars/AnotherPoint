using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnotherPoint.Interfaces;

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
			ClassCore = classCore;
			CtorCore = ctorCore;
			FieldCore = fieldCore;
			InterfaceCore = interfaceCore;
			MethodCore = methodCore;
			PropertyCore = propertyCore;
		}

	    public static IClassCore ClassCore { get; private set; }
		public static ICtorCore CtorCore { get; private set; }
		public static IFieldCore FieldCore { get; private set; }
		public static IInterfaceCore InterfaceCore { get; private set; }
		public static IMethodCore MethodCore { get; private set; }
		public static IPropertyCore PropertyCore { get; private set; }

	    public static void Dispose()
	    {
			ClassCore.Dispose();
			CtorCore.Dispose();
			FieldCore.Dispose();
			InterfaceCore.Dispose();
			MethodCore.Dispose();
			PropertyCore.Dispose();
		}
	}
}

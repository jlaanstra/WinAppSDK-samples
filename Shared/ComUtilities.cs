using Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Win32;
using Windows.Win32.System.Com;

namespace Shared
{
    public static class ComUtilities
    {
        public static void RegisterClass<T>(IClassFactory classFactory)
        {
            RegisterClassObject(typeof(T).GUID, classFactory);
        }

        private static void RegisterClassObject(Guid clsid, object factory)
        {
            int hr = PInvoke.CoRegisterClassObject(in clsid, factory, CLSCTX.CLSCTX_LOCAL_SERVER, (int)REGCLS.REGCLS_MULTIPLEUSE, out uint _);
            if (hr < 0)
            {
                Marshal.ThrowExceptionForHR(hr);
            }
        }
    }
}

using PhoneSync.Shell.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices.ComTypes;

namespace PhoneSync.Shell
{
    [ComVisible(true), Guid("66E8D382-2752-461C-8782-E40633BBECFE")]
    [ProgId("mentalspike.phoneEventHandler")]
    [ComDefaultInterface(typeof(IHWEventHandler))]
    [ClassInterface(ClassInterfaceType.None)]
    public class HardwareEventHandler : IHWEventHandler
    {
        public HardwareEventHandler()
        {
            System.Diagnostics.Debugger.Launch();
        }

        public uint HandleEvent(string pszDeviceID, string pszAltDeviceID, string pszEventType)
        {

            return Hresult.S_OK;
        }

        public uint Initialize(string pszParams)
        {

            return Hresult.S_OK;
        }

        public uint HandleEventWithContent(string pszDeviceID, string pszAltDeviceID, string pszEventType, string pszContentTypeHandler, ref IDataObject pdataobject)
        {
            return Hresult.NOT_IMPLEMENTED;
        }
    }
}

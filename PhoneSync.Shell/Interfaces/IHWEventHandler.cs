using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace PhoneSync.Shell.Interfaces
{
    [Guid("C1FB73D0-EC3A-4BA2-B512-8CDB9187B6D1"), ComImport(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IHWEventHandler
    {
        uint HandleEvent(
            [MarshalAs(UnmanagedType.LPWStr)]string pszDeviceID,
            [MarshalAs(UnmanagedType.LPWStr)]string pszAltDeviceID,
            [MarshalAs(UnmanagedType.LPWStr)]string pszEventType);

        uint Initialize(
            [MarshalAs(UnmanagedType.LPWStr)]string pszParams);

        uint HandleEventWithContent(
            [MarshalAs(UnmanagedType.LPWStr)]string pszDeviceID,
            [MarshalAs(UnmanagedType.LPWStr)]string pszAltDeviceID,
            [MarshalAs(UnmanagedType.LPWStr)]string pszEventType,
            [MarshalAs(UnmanagedType.LPWStr)]string pszContentTypeHandler,
            ref IDataObject pdataobject);
    }
}

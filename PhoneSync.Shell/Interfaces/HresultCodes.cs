using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneSync.Shell.Interfaces
{
    public static class Hresult
    {
        public const uint S_OK = 0x00000000;
        public const uint ABORTED = 0x80004004;
        public const uint NOT_IMPLEMENTED = 0x80004001;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneSync.Models
{
    public struct TransferInfo
    {
        string SourceFile { get; set; }
        string DestinationFile { get; set; }
        TransferStatus Status { get; set; }
    }
}

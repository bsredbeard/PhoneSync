using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneSync.Models
{
    public struct TransferInfo
    {
        public string SourceFile { get; set; }
        public string DestinationFile { get; set; }
        public TransferStatus Status { get; set; }
    }
}

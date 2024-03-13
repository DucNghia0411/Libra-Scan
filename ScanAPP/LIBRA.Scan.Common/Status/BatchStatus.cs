using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.Common.Status
{
    public enum BatchStatus
    {
        New = 1,
        Processing = 2,
        CompleteScan = 3,
        NeedQC = 4,
        CompleteQC = 5
    }
}

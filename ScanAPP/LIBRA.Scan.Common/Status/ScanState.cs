using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.Common.Status
{
    public enum ScanState
    {
        Presession = 0,
        SourceManagerOpen = 1,
        SourceOpen = 2,
        SourceEnable = 3,
        TransferReady = 4,
        Transferring = 5,
        SourceAccquiring = 6,
        SourcePassive = 7,
        SourceEnableSelecting = 8,
    }
}

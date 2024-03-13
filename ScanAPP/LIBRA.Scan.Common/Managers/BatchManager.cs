using LIBRA.Scan.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.Common.Managers
{
    public abstract partial class BatchManager
    {
        public static long? _batchId { get; set; }
        public static string? _batchName { get; set; }
        public static string? _batchPath { get; set; }

        public static void SetBatchId(long batchId)
        {
            _batchId = batchId;
        }

        public static void SetBatchName(string batchName)
        {
            _batchName = batchName;
        }

        public static void SetBatchPath(string batchPath)
        {
            _batchPath = batchPath;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.Common.Managers
{
    public abstract partial class DocumentManager
    {
        public static long? _documentId { get; set; }
        public static string? _documentName { get; set; }
        public static string? _documentPath { get; set; }

        //declare
        public static bool? _documentIsEdit = false;

        public static void SetDocumentId(long documentId)
        {
            _documentId = documentId;
        }

        public static void SetDocumentName(string documentName)
        {
            _documentName = documentName;
        }

        public static void SetDocumentPath(string documentPath)
        {
            _documentPath = documentPath;
        }
    }
}

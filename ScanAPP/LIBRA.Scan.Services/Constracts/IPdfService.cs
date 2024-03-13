using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.Services.Constracts
{
    public interface IPdfService
    {
        void ConvertImageToPdf(string imagePath, string pdfPath);
        void ConvertImagesToPdf(string[] imagePaths, string pdfPath);
        bool ConvertBatchToPdf(long documentId);
    }
}

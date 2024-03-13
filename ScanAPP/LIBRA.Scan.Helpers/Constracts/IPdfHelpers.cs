using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.Helpers.Constracts
{
    public interface IPdfHelpers
    {
        bool ConvertPDFToImages(string filePath, string fileName, int dimOne, int dimTwo);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.Services.Constracts
{
    public interface IProcessService
    {
        Task TransferFile(string sourceFolderPath, string destinationZipPath);
        Task CompressFolder(string sourceFolderPath, string destinationZipPath);
    }
}

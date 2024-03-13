using LIBRA.Scan.Entities.LiteEntities;
using LIBRA.Scan.Services.Constracts;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinSCP;

namespace LIBRA.Scan.Services
{
    public class ProcessService : IProcessService
    {
        private readonly ISftpService _sftpService;

        public ProcessService ()
        {
            _sftpService = new SftpService();
        }

        public async Task TransferFile(string sourceFolderPath, string destinationZipPath)
        {
            if (File.Exists(destinationZipPath))
                File.Delete(destinationZipPath);

            await Task.Run(() => ZipFile.CreateFromDirectory(sourceFolderPath, destinationZipPath));

            SftpConfig config =  await Task.Run(() => _sftpService.Get());

            SessionOptions sessionOptions = new SessionOptions
            {
                Protocol = Protocol.Sftp,
                HostName = config.host_name,
                PortNumber = config.port_number,
                UserName = config.user_name,
                Password = config.password,
                SshHostKeyFingerprint = config.host_key
            };

            string localFilePath = destinationZipPath;
            string remoteDirectory = config.folder_path + Path.GetFileName(sourceFolderPath);

            using (Session session = new Session())
            {
                // Connect
                await Task.Run(() => session.Open(sessionOptions));

                if (System.IO.File.Exists(localFilePath))
                {
                    if (!session.FileExists(remoteDirectory))
                    {
                        await Task.Run(() => session.CreateDirectory(remoteDirectory));
                    }

                    // Send the zip file to the remote server
                    TransferOperationResult transferResult = await Task.Run(() => session.PutFiles(localFilePath, remoteDirectory + "/"));
                    transferResult.Check();

                    // Check if the transfer was successful
                    if (transferResult.IsSuccess)
                    {
                        string remoteZipFilePath = remoteDirectory + "/" + Path.GetFileName(localFilePath);

                        await Task.Run(() => session.ExecuteCommand($"cd {remoteDirectory}; unzip {remoteZipFilePath}"));
                    }
                }
            }
        }

        public async Task CompressFolder(string sourceFolderPath, string destinationZipPath)
        {
            if (File.Exists(destinationZipPath))
                File.Delete(destinationZipPath);

            await Task.Run(() => ZipFile.CreateFromDirectory(sourceFolderPath, destinationZipPath));
        }
    }
}

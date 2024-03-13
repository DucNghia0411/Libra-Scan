using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.AccessControl;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using WinSCP;

namespace LIBRA.Scan.SFTP
{
    public partial class SPTF : ServiceBase
    {
        Timer timer = new Timer();

        public SPTF()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            timer.Elapsed += new ElapsedEventHandler(Process);
            timer.Interval = 10000;
            timer.Enabled = true;
        }

        protected override void OnStop()
        {

        }

        public void OnDebug()
        {
            OnStart(null);
        }

        private void Process(object source, ElapsedEventArgs e)
        {
            MoveFile();
        }

        private void MoveFile()
        {
            timer.Enabled = false;

            SessionOptions sessionOptions = new SessionOptions
            {
                Protocol = Protocol.Sftp,
                HostName = "118.69.224.60",
                PortNumber = 2222,
                UserName = "nghia_sftp",
                Password = "nghiasftp@123",
                SshHostKeyFingerprint = "ssh-ed25519 255 FW5Ds3Hr2Lj70JVMoDujyN1BBfmaujYgi0TEWefxaJw"
            };

            string localFilePath = "C:\\Users\\ADMIN\\Documents\\TestSFTP\\Images.zip";
            string remoteDirectory = "/home/nghia_sftp/LibraScan/";

            using (Session session = new Session())
            {
                // Connect
                session.Open(sessionOptions);


                if (System.IO.File.Exists(localFilePath))
                {
                    // Send the zip file to the remote server
                    TransferOperationResult transferResult = session.PutFiles(localFilePath, remoteDirectory);
                    transferResult.Check();

                    // Check if the transfer was successful
                    if (transferResult.IsSuccess)
                    {
                        string remoteZipFilePath = remoteDirectory + System.IO.Path.GetFileName(localFilePath);
                        session.ExecuteCommand($"cd {remoteDirectory}; unzip {remoteZipFilePath}");
                        //System.IO.File.Delete(localFilePath);
                    }
                    else
                    {
                        // Handle the transfer failure
                        // You can log the error or perform appropriate actions
                    }
                }
                else
                {
                    // Handle the case when the local image file does not exist
                    // You can log the error or perform appropriate actions
                }
            }
        }
    }
}

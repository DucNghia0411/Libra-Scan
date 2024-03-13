namespace LIBRA.Scan.SFTP
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SFTPserviceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.SFTPserviceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // SFTPserviceProcessInstaller
            // 
            this.SFTPserviceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.SFTPserviceProcessInstaller.Password = null;
            this.SFTPserviceProcessInstaller.Username = null;
            // 
            // SFTPserviceInstaller
            // 
            this.SFTPserviceInstaller.ServiceName = "Service1";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.SFTPserviceProcessInstaller,
            this.SFTPserviceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller SFTPserviceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller SFTPserviceInstaller;
    }
}
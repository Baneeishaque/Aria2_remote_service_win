namespace Aria2_Remote_Service
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
            this.serviceProcessInstaller_aria2c_remote = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstaller_aria2c_remote = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstaller_aria2c_remote
            // 
            this.serviceProcessInstaller_aria2c_remote.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstaller_aria2c_remote.Password = null;
            this.serviceProcessInstaller_aria2c_remote.Username = null;
            // 
            // serviceInstaller_aria2c_remote
            // 
            this.serviceInstaller_aria2c_remote.ServiceName = "aria2c_remote";
            this.serviceInstaller_aria2c_remote.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            this.serviceInstaller_aria2c_remote.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceInstaller1_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstaller_aria2c_remote,
            this.serviceInstaller_aria2c_remote});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller_aria2c_remote;
        private System.ServiceProcess.ServiceInstaller serviceInstaller_aria2c_remote;
    }
}
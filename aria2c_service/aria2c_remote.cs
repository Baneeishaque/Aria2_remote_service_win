using Syroot.Windows.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace aria2c_service
{
    public partial class aria2c_remote : ServiceBase
    {
        public aria2c_remote()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = "aria2c.exe";
            startInfo.Arguments = "--enable-rpc --rpc-listen-all --log=aria2c_rpc.log";
            if (Directory.Exists(new KnownFolder(KnownFolderType.Downloads).DefaultPath))
            {
                startInfo.Arguments = startInfo.Arguments + " --dir = " + new KnownFolder(KnownFolderType.Downloads).DefaultPath;
                Process.Start(startInfo);
            }
            else
            {
                try
                {
                    // Try to create the directory.
                    Directory.CreateDirectory(new KnownFolder(KnownFolderType.Downloads).DefaultPath);
                    startInfo.Arguments = startInfo.Arguments + " --dir = " + new KnownFolder(KnownFolderType.Downloads).DefaultPath;
                    Process.Start(startInfo);
                }
                catch (Exception exception)
                {
                    string sSource = "aria2c_rpc";

                    if (!EventLog.SourceExists(sSource))
                        EventLog.CreateEventSource(sSource, "Application");

                    EventLog.WriteEntry(sSource, "Default downloads directory can't be created. Exception is " + exception.ToString(), EventLogEntryType.Error);
                }
            }
        }

        protected override void OnStop()
        {
        }
    }
}

using System.Diagnostics;
using System.ServiceProcess;

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
            startInfo.FileName = @"C:\Programs\aria2-1.33.1-win-64bit-build1\aria2c.exe";

            string arguments = @"--conf-path C:\Programs\aria2_repository\aria2.conf --log=C:\Programs\aria2_repository\aria2_rpc.log";

            //write_event_logs_for_application("aria2c_rpc", "Path : "+new KnownFolder(KnownFolderType.Downloads).Path, EventLogEntryType.Information);
            //write_event_logs_for_application("aria2c_rpc", "Default Path : " + new KnownFolder(KnownFolderType.Downloads).DefaultPath, EventLogEntryType.Information);

            //if (Directory.Exists(new KnownFolder(KnownFolderType.Downloads).Path))
            //{
            //    arguments = arguments + " --dir = " + new KnownFolder(KnownFolderType.Downloads).Path;
            //}
            //else
            //{
            //    try
            //    {
            //        // Try to create the directory.
            //        Directory.CreateDirectory(new KnownFolder(KnownFolderType.Downloads).Path);
            //        arguments = arguments + " --dir=" + new KnownFolder(KnownFolderType.Downloads).Path;
            //    }
            //    catch (Exception exception)
            //    {
            //        write_event_logs_for_application("aria2c_rpc", "Default downloads directory can't be created. Exception is " + exception.ToString(), EventLogEntryType.Error);
            //    }
            //}

            //write_event_logs_for_application("aria2c_rpc", "Arguments " + arguments, EventLogEntryType.Information);
            startInfo.Arguments = arguments;
            Process.Start(startInfo);

        }

        void write_event_logs_for_application(string sSource, string event_message, EventLogEntryType event_type)
        {
            if (!EventLog.SourceExists(sSource))
                EventLog.CreateEventSource(sSource, "Application");

            EventLog.WriteEntry(sSource, event_message, event_type);
        }

        protected override void OnStop()
        {
        }
    }
}

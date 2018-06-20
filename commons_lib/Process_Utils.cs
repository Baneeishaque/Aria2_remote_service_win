using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace commons_lib
{
    public class Process_Utils
    {
        public static int Start_with_arguments(String source, String file, String arguments)
        {
            //TODO : Handle Exception

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                CreateNoWindow = false,
                UseShellExecute = false,

                FileName = file
            };

            Log_Utils.Add_system_event_and_log(source, "File path : " + file, EventLogEntryType.Information);
            Log_Utils.Add_system_event_and_log(source, "File arguments : " + arguments, EventLogEntryType.Information);

            startInfo.Arguments = arguments;

            return Process.Start(startInfo).Id;
        }

        public static bool Kill(String source,int process_id)
        {
            try
            {
                Process proc = Process.GetProcessById(process_id);
                proc.Kill();
                return true;
            }
            catch (Exception ex)
            {
                Log_Utils.Add_system_event_and_log(source, "Kill Exception " + ex.ToString(), EventLogEntryType.Error);
                return false;
            }
        }
    }
}

using commons_service_win_lib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace commons_lib
{
    public class Log_Utils
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void Add_system_event_and_log(string Source, string event_message, EventLogEntryType event_type)
        {
            Service_Utils.Write_event_logs_for_application(Source, event_message, event_type);

            switch (event_type)
            {
                case EventLogEntryType.Information:
                    log.Info(event_message);
                    break;
                case EventLogEntryType.Error:
                    log.Error(event_message);
                    break;
            }

        }
    }
}

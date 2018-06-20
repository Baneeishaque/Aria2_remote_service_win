using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace commons_service_win_lib
{
    public class Service_Utils
    {
        public static void Write_event_logs_for_application(string sSource, string event_message, EventLogEntryType event_type)
        {
            if (!EventLog.SourceExists(sSource))
                EventLog.CreateEventSource(sSource, "Application");

            EventLog.WriteEntry(sSource, event_message, event_type);
        }
    }
}

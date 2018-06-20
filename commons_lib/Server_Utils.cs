using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace commons_lib
{
    public class Server_Utils
    {
        public static int Check_system_status(String source,String URL)
        {
            // 1 - OK, 0 - Error, -1 - Stop Client

            var response = Network_Utils.Get_Request(URL);

            JArray array = JArray.Parse(response);

            int error_status = Network_Utils.Check_error(source, JObject.Parse(array[0].ToString()));

            if (error_status==0)
            {
                if ((Int32)JObject.Parse(array[1].ToString())["system_status"] == 1)
                {
                    Log_Utils.Add_system_event_and_log(source, "System status is OK", EventLogEntryType.Information);
                    return 1;
                }
                else if ((Int32)JObject.Parse(array[1].ToString())["system_status"] == 0)
                {
                    Log_Utils.Add_system_event_and_log(source, "System is in maintanace mode", EventLogEntryType.Information);
                    return -1;
                }
                else
                {
                    Log_Utils.Add_system_event_and_log(source, "Check response, response : " + response, EventLogEntryType.Information);
                }
            }
            else if (error_status == 2)
            {
                Log_Utils.Add_system_event_and_log(source, "Check response, response : " + response, EventLogEntryType.Information);

            }
            
            return 0;
        }
    }
}

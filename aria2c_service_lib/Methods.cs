using aria2_common_lib;
using aria2_rpc_lib;
using commons_lib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace aria2c_service_lib
{
    public class Methods
    {
        static string METHOD_PREFIX = "aria2";
        static string TELL_ACTIVE = "tellActive";
        static string TELL_WAITING = "tellWaiting";
        static string TELL_STOPPED = "tellStopped";

        public static JArray get_active_tasks(String server, String port = "6800", String JOSN_RPC_file_name = "jsonrpc", String access_protocol = "http")
        {
            return Aria2_JSON_RPC_Response_Utils.get_JSON_array_result(Method_Utils_Wrapper.Perform(server, Get_method_full_name(TELL_ACTIVE), new JArray { }, port, JOSN_RPC_file_name, access_protocol));
        }

        public static JArray get_waiting_tasks(String server, int num, int offset, String port = "6800", String JOSN_RPC_file_name = "jsonrpc", String access_protocol = "http")
        {
            return Aria2_JSON_RPC_Response_Utils.get_JSON_array_result(Method_Utils_Wrapper.Perform(server, Get_method_full_name(TELL_WAITING), new JArray { num, offset }, port, JOSN_RPC_file_name, access_protocol));

        }

        public static JArray get_stopped_tasks(String server, int num, int offset, String port = "6800", String JOSN_RPC_file_name = "jsonrpc", String access_protocol = "http")
        {
            return Aria2_JSON_RPC_Response_Utils.get_JSON_array_result(Method_Utils_Wrapper.Perform(server, Get_method_full_name(TELL_STOPPED), new JArray { num, offset }, port, JOSN_RPC_file_name, access_protocol));

        }

        public static IList<Aria2_Task> get_tasks(String server, String port = "6800", String JOSN_RPC_file_name = "jsonrpc", String access_protocol = "http")
        {

            //return JArray.Parse(JToken.FromObject((get_active_tasks(server, port, JOSN_RPC_file_name, access_protocol).Concat(get_waiting_tasks(server, 0, 100, port, JOSN_RPC_file_name, access_protocol))).Concat(get_stopped_tasks(server, 0, 100, port, JOSN_RPC_file_name, access_protocol))).ToString());

            IList<JToken> tasks=JToken.FromObject((get_active_tasks(server, port, JOSN_RPC_file_name, access_protocol).Concat(get_waiting_tasks(server, 0, 100, port, JOSN_RPC_file_name, access_protocol))).Concat(get_stopped_tasks(server, 0, 100, port, JOSN_RPC_file_name, access_protocol))).ToList();

            // serialize JSON results into .NET objects
            IList<Aria2_Task> aria2_tasks = new List<Aria2_Task>();
            foreach (JToken task in tasks)
            {
                // JToken.ToObject is a helper method that uses JsonSerializer internally
                Aria2_Task aria2_task = task.ToObject<Aria2_Task>();
                aria2_tasks.Add(aria2_task);
            }

            return aria2_tasks;

        }

        private static string Get_method_full_name(string method)
        {
            return METHOD_PREFIX + "." + method;
        }
    }
}

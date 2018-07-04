using aria2_common_lib;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace aria2c_JSON_RPC_lib
{
    public class Methods
    {
        static string METHOD_PREFIX = "aria2";
        static string TELL_ACTIVE = "tellActive";
        static string TELL_WAITING = "tellWaiting";
        static string TELL_STOPPED = "tellStopped";
        static string ADD_URI = "addUri";

        public static JArray get_active_tasks(String server, String port = "6800", String JOSN_RPC_file_name = "jsonrpc", String access_protocol = "http")
        {
            return Aria2_JSON_RPC_Response_Utils.get_JSON_array_result(Method_Utils_Wrapper.Perform(server, Get_method_full_name(TELL_ACTIVE), new JArray { }, port, JOSN_RPC_file_name, access_protocol));
        }

        public static string Add_Uri(String url,String server, String port = "6800", String JOSN_RPC_file_name = "jsonrpc", String access_protocol = "http")
        {
            var uris = new JArray
            {
                url
            };

            return Aria2_JSON_RPC_Response_Utils.get_JSON_object_result(Method_Utils_Wrapper.Perform(server, Get_method_full_name(ADD_URI), new JArray {uris}, port, JOSN_RPC_file_name, access_protocol));
        }

        public static JArray get_waiting_tasks(String server, int num, int offset, String port = "6800", String JOSN_RPC_file_name = "jsonrpc", String access_protocol = "http")
        {
            return Aria2_JSON_RPC_Response_Utils.get_JSON_array_result(Method_Utils_Wrapper.Perform(server, Get_method_full_name(TELL_WAITING), new JArray { num, offset }, port, JOSN_RPC_file_name, access_protocol));

        }

        public static JArray get_stopped_tasks(String server, int num, int offset, String port = "6800", String JOSN_RPC_file_name = "jsonrpc", String access_protocol = "http")
        {
            return Aria2_JSON_RPC_Response_Utils.get_JSON_array_result(Method_Utils_Wrapper.Perform(server, Get_method_full_name(TELL_STOPPED), new JArray { num, offset }, port, JOSN_RPC_file_name, access_protocol));

        }

        public static IList<Aria2_RPC_Task> get_tasks(String server, String port = "6800", String JOSN_RPC_file_name = "jsonrpc", String access_protocol = "http")
        {

            //return JArray.Parse(JToken.FromObject((get_active_tasks(server, port, JOSN_RPC_file_name, access_protocol).Concat(get_waiting_tasks(server, 0, 100, port, JOSN_RPC_file_name, access_protocol))).Concat(get_stopped_tasks(server, 0, 100, port, JOSN_RPC_file_name, access_protocol))).ToString());

            IList<JToken> tasks=JToken.FromObject((get_active_tasks(server, port, JOSN_RPC_file_name, access_protocol).Concat(get_waiting_tasks(server, 0, 100, port, JOSN_RPC_file_name, access_protocol))).Concat(get_stopped_tasks(server, 0, 100, port, JOSN_RPC_file_name, access_protocol))).ToList();

            // serialize JSON results into .NET objects
            IList<Aria2_RPC_Task> aria2_tasks = new List<Aria2_RPC_Task>();
            foreach (JToken task in tasks)
            {
                // JToken.ToObject is a helper method that uses JsonSerializer internally
                Aria2_RPC_Task aria2_task = task.ToObject<Aria2_RPC_Task>();
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

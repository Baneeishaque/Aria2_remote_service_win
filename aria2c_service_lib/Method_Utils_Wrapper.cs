using commons_JSON_RPC_lib;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aria2c_JSON_RPC_lib
{
    public class Method_Utils_Wrapper
    {
        static Random random = new Random();

        public static string Perform(String server, string method, JArray requestParams,String port="6800", String JOSN_RPC_file_name = "jsonrpc", String access_protocol = "http")
        {
            return Method_Utils.Perform(server, port, method, random.Next(100).ToString(), requestParams, "2.0", JOSN_RPC_file_name, access_protocol);
        }
    }
}

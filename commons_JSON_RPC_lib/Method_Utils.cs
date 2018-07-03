using commons_JSON_RPC_lib;
using commons_lib;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace aria2c_JSON_RPC_lib
{
    public class Method_Utils
    {
        static WebClient webClient = new WebClient();
        static Random random = new Random();

        public static string Perform(String server, String port, string method, string id,JArray requestParams,string version,String JOSN_RPC_file_name = "jsonrpc", String access_protocol = "http")
        {
            return Network_Utils.Perform_POST(Request_Utils.get_JSON_RPC_path(server, port, JOSN_RPC_file_name, access_protocol), Request_Utils.Create_request(method, id, requestParams,version));
            
        }

    }
}

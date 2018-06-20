using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace commons_JSON_RPC_lib
{
    public class Request_Utils
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //TODO : Use Json RPC lib

        public static string Create_request(string method,string id,JArray requestParams, string version)
        {
            var jsonObject = new JObject
            {
                ["jsonrpc"] = version,
                ["method"] = method,
                ["id"] = id
            };

            if (requestParams.Count > 0)
            {
                jsonObject["params"] = requestParams;
            }

            log.Info(JsonConvert.SerializeObject(jsonObject));
            return JsonConvert.SerializeObject(jsonObject);
        }

        public static string get_JSON_RPC_path(String server,String port,String JOSN_RPC_file_name= "jsonrpc", String access_protocol="http")
        {
            return access_protocol + "://" + server + ":" + port + "/" + JOSN_RPC_file_name;
        }
    }
}

using commons_lib;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aria2_rpc_lib
{
    public class Aria2_JSON_RPC_Response_Utils
    {
        public static JArray get_JSON_array_result(string json_string)
        {
            return JSON_Utils.get_JSON_array_field_value(json_string, "result");
        }
    }
}

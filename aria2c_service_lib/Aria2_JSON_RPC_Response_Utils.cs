using commons_lib;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aria2c_JSON_RPC_lib
{
    public class Aria2_JSON_RPC_Response_Utils
    {
        public static JArray get_JSON_array_result(string json_array_string)
        {
            return JSON_Utils.get_JSON_array_field_value(json_array_string, "result");
        }

        public static string get_JSON_object_result(string json_object_string)
        {
            return JSON_Utils.get_JSON_object_field_value(json_object_string, "result");
        }
    }
}

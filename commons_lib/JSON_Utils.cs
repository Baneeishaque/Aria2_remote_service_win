using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace commons_lib
{
    public class JSON_Utils
    {
        public static JArray get_JSON_array_field_value(string json_string,string JSON_array_field_name)
        {
            return JArray.Parse(JObject.Parse(json_string)[JSON_array_field_name].ToString());
        }
    }
}

using Newtonsoft.Json.Linq;

namespace commons_lib
{
    public class JSON_Utils
    {
        public static JArray get_JSON_array_field_value(string json_array_string,string JSON_array_field_name)
        {
            return JArray.Parse(JObject.Parse(json_array_string)[JSON_array_field_name].ToString());
        }
        public static string get_JSON_object_field_value(string json_object_string, string JSON_object_field_name)
        {
            return JObject.Parse(json_object_string)[JSON_object_field_name].ToString();
        }
    }
}

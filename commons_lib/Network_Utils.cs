using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace commons_lib
{
    public class Network_Utils
    {
        public static string Get_Request(string URL)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static HttpWebResponse Post_Request(string URL, Dictionary<String,String> data_items)
        {
            var request = (HttpWebRequest)WebRequest.Create(URL);

            var postData="";

            for (var i=0;i<data_items.Count;i++)
            {
                KeyValuePair<string, string> kvp = data_items.ElementAt(i);

                if (i==0)
                {
                    postData = postData + kvp.Key + "=" + kvp.Value;
                }
                else
                {
                    postData = postData + "&"+kvp.Key + "=" + kvp.Value;
                }
            }

            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            return (HttpWebResponse)request.GetResponse();
        }

        public static string Get_parametered_URL(string base_URL, List<KeyValuePair<string, string>> data_items)
        {
            for (int i = 0; i < data_items.Count; i++)
            {
                KeyValuePair<string, string> kvp = data_items[i];
                if (i == 0)
                {
                    base_URL = base_URL + "?";
                }
                else
                {
                    base_URL = base_URL + "&";
                }
                base_URL = base_URL + kvp.Key + "=" + kvp.Value;
            }

            return base_URL;
        }

        public static int Check_error(string source,JObject error_node)
        {
            //TODO : Use interface for success
            if ((Int32)error_node["error_status"] == 0)
            {
                return 0;
            }
            else if ((Int32)error_node["error_status"] == 1)
            {
                Log_Utils.Add_system_event_and_log(source, "Error : " + error_node["error"] + " - " + error_node["error_number"], EventLogEntryType.Information);
                return 1;
            }
            else
            {
                return 2;
            }
        }

        static WebClient webClient = new WebClient();
        public static string Perform_POST(string URL, string data)
        {
            return webClient.UploadString(URL, "POST", data);
        }
    }

    
}

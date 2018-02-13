using EasyHttp.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;

namespace Tests
{
    class Program
    {
        static HttpClient http_client = new HttpClient();
        static void Main(string[] args)
        {
            if(check_system_status())
            {
                get_Tasks();
            }
            
        }

        public static void get_Tasks()
        {
            //Console.WriteLine(Environment.MachineName);
            var response = http_client.Get(API.get_API(API.select_Tasks), new { host = Environment.MachineName });
            Console.WriteLine(response.RawText);
            Console.ReadKey();
            List<Task> tasks = JsonConvert.DeserializeObject<List<Task>>(response.RawText);
            var webClient = new WebClient();
            for (int i=0;i<tasks.Count;i++)
            {
                var add_response = webClient.UploadString("http://localhost:6800/jsonrpc", "POST", create_json_request(tasks[i].url,tasks[i].id));
                Console.WriteLine(add_response);
                Console.ReadKey();
            }
        }

        public static string create_json_request(string url,string id)
        {
            var jsonObject = new JObject();
            jsonObject["jsonrpc"] = "2.0";
            jsonObject["method"] = "aria2.addUri";
            jsonObject["id"] = id;
            var requestParams = new JArray();
            var uris = new JArray();
            uris.Add(url);
            requestParams.Add(uris);
            jsonObject["params"] = requestParams;
            return JsonConvert.SerializeObject(jsonObject);
        }

        private static bool check_system_status()
        {
            //Console.WriteLine(API.get_API(API.select_Configuration));
            var response = http_client.Get(API.get_API(API.select_Configuration));
            //Console.WriteLine(response.RawText);
            //Console.ReadKey();

            //List<Configuration> list = JsonConvert.DeserializeObject<List<Configuration>>(response.RawText);
            //Console.WriteLine(list[0].error_status);
            //Console.WriteLine(list[0].system_status);
            //Console.WriteLine(list[1].error_status);
            //Console.WriteLine(list[1].system_status);
            //Console.ReadKey();

            //var obj = JsonConvert.DeserializeObject(response.RawText);
            //Console.Write(obj);
            //Console.ReadKey();

            JArray array = JArray.Parse(response.RawText);

            //Console.WriteLine(array);
            //Console.WriteLine(array[0]);
            //Console.WriteLine(JObject.Parse(array[0].ToString())["error_status"]);

            if ((Int32)JObject.Parse(array[0].ToString())["error_status"] == 0)
            {
                if ((Int32)JObject.Parse(array[1].ToString())["system_status"] == 1)
                {
                    Console.WriteLine("System status is OK");
                    return true;
                }
                else if ((Int32)JObject.Parse(array[1].ToString())["system_status"] == 0)
                {
                    Console.WriteLine("System is in maintanace mode");
                }
                else
                {
                    Console.WriteLine("Check response");
                }

            }
            else
            {
                Console.WriteLine("Error : " + JObject.Parse(array[0].ToString())["error"] + " - " + JObject.Parse(array[0].ToString())["error_number"]);
            }
            return false;
        }
    }
}

using EasyHttp.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace Tests
{
    class Program
    {
        static EasyHttp.Http.HttpClient http_client = new EasyHttp.Http.HttpClient();
        static WebClient webClient = new WebClient();
        static void Main(string[] args)
        {
            // Create a timer
            //System.Timers.Timer myTimer = new System.Timers.Timer();
            //// Tell the timer what to do when it elapses
            //myTimer.Elapsed += new ElapsedEventHandler(aria2c_service_main);
            //// Set it to go off every 1 minutes
            //myTimer.Interval = 1 * 60 * 1000;
            //// And start it        
            //myTimer.Enabled = true;

            while (true)
            {
                aria2c_service_main();
                Thread.Sleep(1 * 60 * 1000);
            }
        }

        public static void aria2c_service_main()
        {
            if (check_system_status())
            {
                get_Tasks();
            }
        }

        public static void get_Tasks()
        {
            //Console.WriteLine(Environment.MachineName);
            var get_response = http_client.Get(API.get_API(API.select_Tasks), new { host = Environment.MachineName });
            Console.WriteLine(get_response.RawText);
            Console.ReadKey();


            JArray array = JArray.Parse(get_response.RawText);
            if ((Int32)JObject.Parse(array[0].ToString())["error_status"] == 1)
            {
                Console.WriteLine("Error : " + JObject.Parse(array[0].ToString())["error"] + " - " + JObject.Parse(array[0].ToString())["error_number"]);
            }
            else if ((Int32)JObject.Parse(array[0].ToString())["error_status"] == 2)
            {
                Console.WriteLine("No Tasks");
            }
            else if ((Int32)JObject.Parse(array[0].ToString())["error_status"] == 0)
            {
                //List<Task> tasks = JsonConvert.DeserializeObject<List<Task>>(get_response.RawText);

                List<Task> tasks;
                for (int i = 0; i < tasks.Count; i++)
                {

                }
                    for (int i = 0; i < tasks.Count; i++)
                {
                    var add_response = webClient.UploadString("http://localhost:6800/jsonrpc", "POST", create_json_request(tasks[i].url, tasks[i].id));
                    Console.WriteLine(add_response);
                    Console.ReadKey();

                    JObject json_object = JObject.Parse(add_response);

                    var update_response = http_client.Post(API.get_API(API.update_Task), new { id = tasks[i].id, gid = json_object["result"] }, HttpContentTypes.ApplicationXWwwFormUrlEncoded);
                    Console.WriteLine(update_response.RawText);
                    Console.ReadKey();

                    array = JArray.Parse(update_response.RawText);

                    if ((Int32)JObject.Parse(array[0].ToString())["error_status"] == 0)
                    {
                        Console.WriteLine("Task updated successfully");
                    }
                    else if ((Int32)JObject.Parse(array[0].ToString())["error_status"] == 1)
                    {
                        Console.WriteLine("Error : " + JObject.Parse(array[0].ToString())["error"] + " - " + JObject.Parse(array[0].ToString())["error_number"]);
                    }
                    else
                    {
                        Console.WriteLine("Check response");
                    }
                }
            }
            else
            {
                Console.WriteLine("Check response");
            }
        }

        public static string create_json_request(string url, string id)
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
            var response = http_client.Get(API.get_API(API.select_Configuration));

            JArray array = JArray.Parse(response.RawText);

            if ((Int32)JObject.Parse(array[0].ToString())["error_status"] == 0)
            {
                if ((Int32)JObject.Parse(array[1].ToString())["system_status"] == 1)
                {
                    Console.WriteLine("System status is OK");
                    update_host();
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
            else if ((Int32)JObject.Parse(array[0].ToString())["error_status"] == 0)
            {
                Console.WriteLine("Error : " + JObject.Parse(array[0].ToString())["error"] + " - " + JObject.Parse(array[0].ToString())["error_number"]);
            }
            else
            {
                Console.WriteLine("Check response");
            }
            return false;
        }

        private static void update_host()
        {
            //Console.WriteLine(Environment.MachineName);
            //var update_response = http_client.Post(API.get_API(API.update_Host), new { name = Environment.MachineName }, HttpContentTypes.ApplicationJson);

            var client = new System.Net.Http.HttpClient();

            var pairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("name", Environment.MachineName)
            };

            var content = new FormUrlEncodedContent(pairs);

            var update_response = client.PostAsync(API.get_API(API.update_Host), content).Result;

            if (update_response.IsSuccessStatusCode)
            {
                //Console.WriteLine(update_response.Content.ReadAsStringAsync().Result);
                //Console.ReadKey();

                if ((Int32)JObject.Parse(update_response.Content.ReadAsStringAsync().Result)["error_status"] == 0)
                {
                    Console.WriteLine("Host updated successfully");
                }
                else if ((Int32)JObject.Parse(update_response.Content.ReadAsStringAsync().Result)["error_status"] == 1)
                {
                    Console.WriteLine("Error : " + JObject.Parse(update_response.Content.ReadAsStringAsync().Result)["error"] + " - " + JObject.Parse(update_response.Content.ReadAsStringAsync().Result)["error_number"]);
                }
                else
                {
                    Console.WriteLine("Check response");
                }

            }


        }

    }
}


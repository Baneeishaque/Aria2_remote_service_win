using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
            Console.WriteLine(Environment.MachineName+" Sync. Started...");
            Console.WriteLine(API.get_API(API.select_Tasks));
            var get_response = http_client.Get(API.get_API(API.select_Tasks), new { host = Environment.MachineName });
            Console.WriteLine("New Tasks : "+get_response.RawText);
            //Console.ReadKey();


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
                List<Task> tasks=new List<Task>();
                for (int i = 1; i < array.Count; i++)
                {
                    JObject json_Task = JObject.Parse(array[i].ToString());
                    Task current_task = new Task();
                    current_task.id = (String)json_Task["id"];
                    current_task.url = (String)json_Task["url"];
                    tasks.Add(current_task);

                    Console.WriteLine("ID : "+current_task.id+", Task : " + current_task.url);
                    Console.WriteLine("Request : "+create_json_request(current_task.url, current_task.id));
                    //Console.ReadKey();

                    var add_response = webClient.UploadString("http://localhost:6800/jsonrpc", "POST", create_json_request(current_task.url, current_task.id));
                    Console.WriteLine("Task Addition Response : "+add_response);
                    //Console.ReadKey();

                    JObject json_object = JObject.Parse(add_response);

                    update_task(current_task.id, json_object["result"].ToString());
                    
                }

            }
            else
            {
                Console.WriteLine("Check response");
            }
        }

        private static void update_task(String id,String gid)
        {

            var client = new System.Net.Http.HttpClient();

            var pairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("id", id),
                new KeyValuePair<string, string>("gid", gid)
            };

            var content = new FormUrlEncodedContent(pairs);

            var update_response = client.PostAsync(API.get_API(API.update_Task), content).Result;

            if (update_response.IsSuccessStatusCode)
            {
                Console.WriteLine(update_response.Content.ReadAsStringAsync().Result);
                //Console.ReadKey();

                if ((Int32)JObject.Parse(update_response.Content.ReadAsStringAsync().Result)["error_status"] == 0)
                {
                    Console.WriteLine("Task updated successfully");
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


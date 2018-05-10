using aria2c_service;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace Tests
{
    class Program
    {
        static EasyHttp.Http.HttpClient http_client = new EasyHttp.Http.HttpClient();
        static WebClient webClient = new WebClient();
        static void Main(string[] args)
        {
            Console.WriteLine("Aria2 exe path : " + ConfigurationManager.AppSettings["aria2c_HOME"] + "\\aria2c.exe");
            //Console.ReadKey();
            while (true)
            {
                Aria2c_service_main();
                Thread.Sleep(1 * 60 * 1000);
            }
        }

        public static void Aria2c_service_main()
        {
            if (Check_system_status())
            {
                Get_Tasks();
            }
        }


        public static void Get_Tasks()
        {
            Console.WriteLine(Environment.MachineName + " Sync. Started...");
            Console.WriteLine(API.get_API(API.select_Tasks));
            var get_response = http_client.Get(API.get_API(API.select_Tasks), new { host = Environment.MachineName });
            Console.WriteLine("New Tasks : " + get_response.RawText);
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
                List<Task> tasks = new List<Task>();
                for (int i = 1; i < array.Count; i++)
                {
                    JObject json_Task = JObject.Parse(array[i].ToString());
                    Task current_task = new Task();
                    current_task.id = (String)json_Task["id"];
                    current_task.url = (String)json_Task["url"];
                    tasks.Add(current_task);

                    Console.WriteLine("ID : " + current_task.id + ", Task : " + current_task.url);
                    Console.WriteLine("Request : " + Create_json_request_addUri(current_task.url, current_task.id));
                    //Console.ReadKey();

                    var add_response = webClient.UploadString("http://localhost:6800/jsonrpc", "POST", Create_json_request_addUri(current_task.url, current_task.id));
                    Console.WriteLine("Task Addition Response : " + add_response);
                    //Console.ReadKey();

                    JObject json_object = JObject.Parse(add_response);

                    Update_task_gid(current_task.id, json_object["result"].ToString());
                }
            }
            else
            {
                Console.WriteLine("Check response");
            }
        }

        private static void Update_task_gid(String id, String gid)
        {

            var client = new HttpClient();

            var pairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("id", id),
                new KeyValuePair<string, string>("gid", gid)
            };

            var content = new FormUrlEncodedContent(pairs);

            var update_response = client.PostAsync(API.get_API(API.update_Task_gid), content).Result;

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

        private static void Update_task_current_status(String id, String current_status)
        {

            var client = new HttpClient();

            var pairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("id", id),
                new KeyValuePair<string, string>("current_status", current_status)
            };

            var content = new FormUrlEncodedContent(pairs);

            var update_response = client.PostAsync(API.get_API(API.update_Task_current_status), content).Result;

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
                    Console.WriteLine("Check response, response : " + update_response);
                }

            }
        }

        public static string Create_json_request_addUri(string url, string id)
        {
            var jsonObject = new JObject
            {
                ["jsonrpc"] = "2.0",
                ["method"] = "aria2.addUri",
                ["id"] = id
            };
            var requestParams = new JArray();
            var uris = new JArray
            {
                url
            };
            requestParams.Add(uris);
            jsonObject["params"] = requestParams;
            return JsonConvert.SerializeObject(jsonObject);
        }

        public static string Create_json_request_tellStatus(string gid, string id)
        {
            var jsonObject = new JObject
            {
                ["jsonrpc"] = "2.0",
                ["method"] = "aria2.tellStatus",
                ["id"] = id
            };
            var requestParams = new JArray
            {
                gid
            };
            jsonObject["params"] = requestParams;
            return JsonConvert.SerializeObject(jsonObject);
        }

        private static bool Check_system_status()
        {
            var response = http_client.Get(API.get_API(API.select_Configuration));

            JArray array = JArray.Parse(response.RawText);

            if ((Int32)JObject.Parse(array[0].ToString())["error_status"] == 0)
            {
                if ((Int32)JObject.Parse(array[1].ToString())["system_status"] == 1)
                {
                    Console.WriteLine("System status is OK");
                    Update_host();
                    Update_tasks();
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

        private static void Update_tasks()
        {
            Console.WriteLine(Environment.MachineName + " Sync. Started...");
            Console.WriteLine("API URL : " + API.get_API(API.select_Task_gids));
            var get_response = http_client.Get(API.get_API(API.select_Task_gids), new { host = Environment.MachineName });
            Console.WriteLine("Existing Tasks : " + get_response.RawText);
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
                List<Task> tasks = new List<Task>();
                for (int i = 1; i < array.Count; i++)
                {
                    JObject json_Task = JObject.Parse(array[i].ToString());
                    Task current_task = new Task
                    {
                        id = (String)json_Task["id"],
                        gid = (String)json_Task["gid"]
                    };
                    tasks.Add(current_task);

                    Console.WriteLine("ID : " + current_task.id + ", gid : " + current_task.gid);
                    Console.WriteLine("Request : " + Create_json_request_tellStatus(current_task.gid, current_task.id));
                    Console.ReadKey();

                    try
                    {
                        var status_response = webClient.UploadString("http://localhost:6800/jsonrpc", "POST", Create_json_request_tellStatus(current_task.gid, current_task.id));
                        Console.WriteLine("Task tellStatus Response : " + status_response);
                        Console.ReadKey();

                        JObject json_object = JObject.Parse(status_response);

                        Update_task_current_status(current_task.id, json_object.ToString());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Task exception : " + e.ToString());
                    }
                }
            }
            else
            {
                Console.WriteLine("Check response, response : " + get_response);
            }
        }

        private static void Update_host()
        {
            var request = (HttpWebRequest)WebRequest.Create(API.get_API(API.update_Host));

            var postData = "name=" + Environment.MachineName;
            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Console.WriteLine("Host update response : " + responseString);
                //Console.ReadKey();

                if ((Int32)JObject.Parse(responseString)["error_status"] == 0)
                {
                    Console.WriteLine("Host updated successfully");
                }
                else if ((Int32)JObject.Parse(responseString)["error_status"] == 1)
                {
                    Console.WriteLine("Error : " + JObject.Parse(responseString)["error"] + " - " + JObject.Parse(responseString)["error_number"]);
                }
                else
                {
                    Console.WriteLine("Check response");
                }
            }
        }
    }
}


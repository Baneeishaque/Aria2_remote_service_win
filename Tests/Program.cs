using aria2_client_lib;
using aria2_common;
using commons_server_client_lib;
using System;
using System.Net;

namespace Tests
{
    class Program : IService_Client_Utils
    {
        //static EasyHttp.Http.HttpClient http_client = new EasyHttp.Http.HttpClient();
        //static WebClient webClient = new WebClient();

        private readonly IService_Client_Utils aria2_client_interface = new Aria2_Client(API_Wrapper.get_API(Aria2_Remote_API_Constants.SELECT_CONFIGURATION), API_Wrapper.get_API(Aria2_Remote_API_Constants.UPDATE_HOST), Aria2_RPC_Constants.HOST);

        static void Main(string[] args)
        {
            Program p = new Program();
            p.Start_client();

            Console.ReadKey();
        }

        public int Start_client()
        {
            return aria2_client_interface.Start_client();
        }

        public bool Stop_client(int client_process_id)
        {
            return aria2_client_interface.Stop_client(client_process_id);
        }

        //public static void Aria2c_service_main()
        //{
        //    if (Check_system_status())
        //    {
        //        Get_Tasks();
        //    }
        //}


        //public static void Get_Tasks()
        //{
        //    Console.WriteLine(Environment.MachineName + " Sync. Started...");
        //    Console.WriteLine(API_Wrapper.get_API(API_Wrapper.SELECT_TASKS));
        //    var get_response = http_client.Get(API_Wrapper.get_API(API_Wrapper.SELECT_TASKS), new { host = Environment.MachineName });
        //    Console.WriteLine("New Tasks : " + get_response.RawText);
        //    //Console.ReadKey();

        //    JArray array = JArray.Parse(get_response.RawText);
        //    if ((Int32)JObject.Parse(array[0].ToString())["error_status"] == 1)
        //    {
        //        Console.WriteLine("Error : " + JObject.Parse(array[0].ToString())["error"] + " - " + JObject.Parse(array[0].ToString())["error_number"]);
        //    }
        //    else if ((Int32)JObject.Parse(array[0].ToString())["error_status"] == 2)
        //    {
        //        Console.WriteLine("No Tasks");
        //    }
        //    else if ((Int32)JObject.Parse(array[0].ToString())["error_status"] == 0)
        //    {
        //        List<Task> tasks = new List<Task>();
        //        for (int i = 1; i < array.Count; i++)
        //        {
        //            JObject json_Task = JObject.Parse(array[i].ToString());
        //            Task current_task = new Task();
        //            current_task.id = (String)json_Task["id"];
        //            current_task.url = (String)json_Task["url"];
        //            tasks.Add(current_task);

        //            Console.WriteLine("ID : " + current_task.id + ", Task : " + current_task.url);
        //            Console.WriteLine("Request : " + Create_json_request_addUri(current_task.url, current_task.id));
        //            //Console.ReadKey();

        //            var add_response = webClient.UploadString("http://localhost:6800/jsonrpc", "POST", Create_json_request_addUri(current_task.url, current_task.id));
        //            Console.WriteLine("Task Addition Response : " + add_response);
        //            //Console.ReadKey();

        //            JObject json_object = JObject.Parse(add_response);

        //            Update_task_gid(current_task.id, json_object["result"].ToString());
        //        }
        //    }
        //    else
        //    {
        //        Console.WriteLine("Check response");
        //    }
        //}

        //private static void Update_task_gid(String id, String gid)
        //{

        //    var client = new HttpClient();

        //    var pairs = new List<KeyValuePair<string, string>>
        //    {
        //        new KeyValuePair<string, string>("id", id),
        //        new KeyValuePair<string, string>("gid", gid)
        //    };

        //    var content = new FormUrlEncodedContent(pairs);

        //    var update_response = client.PostAsync(API_Wrapper.get_API(API_Wrapper.UPDATE_TASK_GID), content).Result;

        //    if (update_response.IsSuccessStatusCode)
        //    {
        //        Console.WriteLine(update_response.Content.ReadAsStringAsync().Result);
        //        //Console.ReadKey();

        //        if ((Int32)JObject.Parse(update_response.Content.ReadAsStringAsync().Result)["error_status"] == 0)
        //        {
        //            Console.WriteLine("Task updated successfully");
        //        }
        //        else if ((Int32)JObject.Parse(update_response.Content.ReadAsStringAsync().Result)["error_status"] == 1)
        //        {
        //            Console.WriteLine("Error : " + JObject.Parse(update_response.Content.ReadAsStringAsync().Result)["error"] + " - " + JObject.Parse(update_response.Content.ReadAsStringAsync().Result)["error_number"]);
        //        }
        //        else
        //        {
        //            Console.WriteLine("Check response");
        //        }

        //    }
        //}

        //private static void Update_task_current_status(String id, String current_status)
        //{

        //    var client = new HttpClient();

        //    var pairs = new List<KeyValuePair<string, string>>
        //    {
        //        new KeyValuePair<string, string>("id", id),
        //        new KeyValuePair<string, string>("current_status", current_status)
        //    };

        //    var content = new FormUrlEncodedContent(pairs);

        //    var update_response = client.PostAsync(API_Wrapper.get_API(API_Wrapper.UPDATE_TASK_CURRENT_STATUS), content).Result;

        //    if (update_response.IsSuccessStatusCode)
        //    {
        //        Console.WriteLine(update_response.Content.ReadAsStringAsync().Result);
        //        //Console.ReadKey();

        //        if ((Int32)JObject.Parse(update_response.Content.ReadAsStringAsync().Result)["error_status"] == 0)
        //        {
        //            Console.WriteLine("Task updated successfully");
        //        }
        //        else if ((Int32)JObject.Parse(update_response.Content.ReadAsStringAsync().Result)["error_status"] == 1)
        //        {
        //            Console.WriteLine("Error : " + JObject.Parse(update_response.Content.ReadAsStringAsync().Result)["error"] + " - " + JObject.Parse(update_response.Content.ReadAsStringAsync().Result)["error_number"]);
        //        }
        //        else
        //        {
        //            Console.WriteLine("Check response, response : " + update_response);
        //        }

        //    }
        //}

        //public static string Create_json_request_addUri(string url, string id)
        //{
        //    var jsonObject = new JObject
        //    {
        //        ["jsonrpc"] = "2.0",
        //        ["method"] = "aria2.addUri",
        //        ["id"] = id
        //    };
        //    var requestParams = new JArray();
        //    var uris = new JArray
        //    {
        //        url
        //    };
        //    requestParams.Add(uris);
        //    jsonObject["params"] = requestParams;
        //    return JsonConvert.SerializeObject(jsonObject);
        //}

        //public static string Create_json_request_tellStatus(string gid, string id)
        //{
        //    var jsonObject = new JObject
        //    {
        //        ["jsonrpc"] = "2.0",
        //        ["method"] = "aria2.tellStatus",
        //        ["id"] = id
        //    };
        //    var requestParams = new JArray
        //    {
        //        gid
        //    };
        //    jsonObject["params"] = requestParams;
        //    return JsonConvert.SerializeObject(jsonObject);
        //}

        //private static bool Check_system_status()
        //{
        //    var response = http_client.Get(API_Wrapper.get_API(API_Wrapper.SELECT_CONFIGURATION));

        //    JArray array = JArray.Parse(response.RawText);

        //    if ((Int32)JObject.Parse(array[0].ToString())["error_status"] == 0)
        //    {
        //        if ((Int32)JObject.Parse(array[1].ToString())["system_status"] == 1)
        //        {
        //            Console.WriteLine("System status is OK");
        //            Update_host();
        //            Update_tasks();
        //            return true;
        //        }
        //        else if ((Int32)JObject.Parse(array[1].ToString())["system_status"] == 0)
        //        {
        //            Console.WriteLine("System is in maintanace mode");
        //        }
        //        else
        //        {
        //            Console.WriteLine("Check response");
        //        }

        //    }
        //    else if ((Int32)JObject.Parse(array[0].ToString())["error_status"] == 0)
        //    {
        //        Console.WriteLine("Error : " + JObject.Parse(array[0].ToString())["error"] + " - " + JObject.Parse(array[0].ToString())["error_number"]);
        //    }
        //    else
        //    {
        //        Console.WriteLine("Check response");
        //    }
        //    return false;
        //}

        //private static void Update_tasks()
        //{
        //    Console.WriteLine(Environment.MachineName + " Sync. Started...");
        //    Console.WriteLine("API URL : " + API_Wrapper.get_API(API_Wrapper.SELECT_TASK_GIDS));
        //    var get_response = http_client.Get(API_Wrapper.get_API(API_Wrapper.SELECT_TASK_GIDS), new { host = Environment.MachineName });
        //    Console.WriteLine("Existing Tasks : " + get_response.RawText);
        //    Console.ReadKey();

        //    JArray array = JArray.Parse(get_response.RawText);
        //    if ((Int32)JObject.Parse(array[0].ToString())["error_status"] == 1)
        //    {
        //        Console.WriteLine("Error : " + JObject.Parse(array[0].ToString())["error"] + " - " + JObject.Parse(array[0].ToString())["error_number"]);
        //    }
        //    else if ((Int32)JObject.Parse(array[0].ToString())["error_status"] == 2)
        //    {
        //        Console.WriteLine("No Tasks");
        //    }
        //    else if ((Int32)JObject.Parse(array[0].ToString())["error_status"] == 0)
        //    {
        //        List<Task> tasks = new List<Task>();
        //        for (int i = 1; i < array.Count; i++)
        //        {
        //            JObject json_Task = JObject.Parse(array[i].ToString());
        //            Task current_task = new Task
        //            {
        //                id = (String)json_Task["id"],
        //                gid = (String)json_Task["gid"]
        //            };
        //            tasks.Add(current_task);

        //            Console.WriteLine("ID : " + current_task.id + ", gid : " + current_task.gid);
        //            Console.WriteLine("Request : " + Create_json_request_tellStatus(current_task.gid, current_task.id));
        //            Console.ReadKey();

        //            try
        //            {
        //                var status_response = webClient.UploadString("http://localhost:6800/jsonrpc", "POST", Create_json_request_tellStatus(current_task.gid, current_task.id));
        //                Console.WriteLine("Task tellStatus Response : " + status_response);
        //                Console.ReadKey();

        //                JObject json_object = JObject.Parse(status_response);

        //                Update_task_current_status(current_task.id, json_object.ToString());
        //            }
        //            catch (Exception e)
        //            {
        //                Console.WriteLine("Task exception : " + e.ToString());
        //            }
        //        }
        //    }
        //    else
        //    {
        //        Console.WriteLine("Check response, response : " + get_response);
        //    }
        //}

        //private static void Update_host()
        //{
        //    var request = (HttpWebRequest)WebRequest.Create(API_Wrapper.get_API(API_Wrapper.UPDATE_HOST));

        //    var postData = "name=" + Environment.MachineName;
        //    var data = Encoding.ASCII.GetBytes(postData);

        //    request.Method = "POST";
        //    request.ContentType = "application/x-www-form-urlencoded";
        //    request.ContentLength = data.Length;

        //    using (var stream = request.GetRequestStream())
        //    {
        //        stream.Write(data, 0, data.Length);
        //    }

        //    var response = (HttpWebResponse)request.GetResponse();

        //    var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

        //    if (response.StatusCode == HttpStatusCode.OK)
        //    {
        //        Console.WriteLine("Host update response : " + responseString);
        //        //Console.ReadKey();

        //        if ((Int32)JObject.Parse(responseString)["error_status"] == 0)
        //        {
        //            Console.WriteLine("Host updated successfully");
        //        }
        //        else if ((Int32)JObject.Parse(responseString)["error_status"] == 1)
        //        {
        //            Console.WriteLine("Error : " + JObject.Parse(responseString)["error"] + " - " + JObject.Parse(responseString)["error_number"]);
        //        }
        //        else
        //        {
        //            Console.WriteLine("Check response");
        //        }
        //    }
        //}
    }
}


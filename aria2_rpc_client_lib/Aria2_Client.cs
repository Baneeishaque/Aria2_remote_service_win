using aria2_client_lib;
using commons_server_client_lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Diagnostics;
using commons_lib;
using System.Timers;
using aria2_common;
using System.Net;
using System.IO;
using aria2_rpc_lib;
using Newtonsoft.Json.Linq;
using aria2c_JSON_RPC_lib;
using Newtonsoft.Json;
using aria2_common_lib;

namespace aria2_client_lib
{
    public class Aria2_Client : Aria2_Client_Wrapper
    {
        private static int secondsElapsed = 0, aria2c_process_id;

        //This timer willl run the process at the interval specified(currently 1 minute) once enabled
        //Timer timer = new Timer(1000 * 60);
        Timer timer = new Timer(1000 * 10);

        bool idle_flag = true;

        public string ARIA2_EXE, CONFIGURATION_URL, UPDATE_HOST_URL, ARIA2_HOST, ARIA2_PORT, ARIA2_JSON_RPC_FILE_NAME, ARIA2_ACCESS_PROTOCOL;
        public string ARIA2_CONFIG_FILE;
        public string ARIA2_RPC_LOG_FILE;

        public Aria2_Client(string CONFIGURATION_URL, string UPDATE_HOST_URL, string ARIA2_HOST, string ARIA2_EXE = "aria2c.exe", string ARIA2_CONFIG_FILE = "aria2.conf", string ARIA2_RPC_LOG_FILE = "aria2_rpc.log", string ARIA2_PORT = "6800", string ARIA2_JSON_RPC_FILE_NAME = "jsonrpc", string ARIA2_ACCESS_PROTOCOL = "http")
        {
            this.CONFIGURATION_URL = CONFIGURATION_URL;
            this.UPDATE_HOST_URL = UPDATE_HOST_URL;
            this.ARIA2_HOST = ARIA2_HOST;
            this.ARIA2_EXE = ARIA2_EXE;
            this.ARIA2_CONFIG_FILE = ARIA2_CONFIG_FILE;
            this.ARIA2_RPC_LOG_FILE = ARIA2_RPC_LOG_FILE;
            this.ARIA2_PORT = ARIA2_PORT;
            this.ARIA2_JSON_RPC_FILE_NAME = ARIA2_JSON_RPC_FILE_NAME;
            this.ARIA2_ACCESS_PROTOCOL = ARIA2_ACCESS_PROTOCOL;
        }

        public override int Start_Aria2()
        {
            //TODO : Check Internet Connectivity

            //startInfo.FileName = @"C:\Programs\aria2-1.33.1-win-64bit-build1\aria2c.exe";
            //string arguments = @"--conf-path C:\Programs\aria2_repository\aria2.conf --log=C:\Programs\aria2_repository\aria2_rpc.log";s

            string arguments = @"--conf-path " + ConfigurationManager.AppSettings["aria2_repository"] + "\\" + ARIA2_CONFIG_FILE + " --log=" + ConfigurationManager.AppSettings["aria2_repository"] + "\\" + ARIA2_RPC_LOG_FILE;

            //Log_Utils.Add_system_event_and_log(Aria2_Client_Constants.EVENT_SOURCE, "Aria2 arguments : " + arguments, EventLogEntryType.Information);

            aria2c_process_id = Process_Utils.Start_with_arguments(Aria2_Client_Constants.EVENT_SOURCE, @ConfigurationManager.AppSettings["aria2_HOME"] + "\\" + ARIA2_EXE, arguments);

            // point the timer elapsed to the handler
            timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);

            // turn on the timer
            timer.Enabled = true;

            return aria2c_process_id;
        }

        public override bool Stop_Aria2(int aria2_process_id)
        {
            timer.Enabled = false;

            return Process_Utils.Kill(Aria2_Client_Constants.EVENT_SOURCE, aria2_process_id);
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (idle_flag)
            {
                Aria2_remote();
            }
        }

        public void Aria2_remote()
        {
            idle_flag = false;

            secondsElapsed += 10;

            //TODO: Check System Updation

            int server_status = Server_Utils.Check_system_status(Aria2_Client_Constants.EVENT_SOURCE, CONFIGURATION_URL);

            if (server_status == 1)
            {
                Update_host();
                Update_tasks();
                Get_Tasks();
            }
            else if (server_status == -1)
            {
                Stop_Aria2(aria2c_process_id);
            }

            idle_flag = true;
        }

        private void Update_host()
        {
            //TODO : Update Drive free spaces

            Dictionary<string, string> data_items = new Dictionary<string, string>() { { "name", Environment.MachineName } };

            var response = Network_Utils.Post_Request(UPDATE_HOST_URL, data_items);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                //Log_Utils.Add_system_event_and_log("aria2c_rpc", "Host update response : " + responseString, EventLogEntryType.Information);

                int error_status = Network_Utils.Check_error(Aria2_Client_Constants.EVENT_SOURCE, JObject.Parse(responseString));

                if (error_status == 0)
                {
                    Log_Utils.Add_system_event_and_log(Aria2_Client_Constants.EVENT_SOURCE, "Host updated successfully", EventLogEntryType.Information);
                }
                else if (error_status == 2)
                {
                    Log_Utils.Add_system_event_and_log(Aria2_Client_Constants.EVENT_SOURCE, "Check response, response : " + response, EventLogEntryType.Information);
                }
            }
        }

        private void Update_tasks()
        {
            Log_Utils.Add_system_event_and_log(Aria2_Client_Constants.EVENT_SOURCE, Environment.MachineName + " Updating task status...", EventLogEntryType.Information);

            IList<Aria2_Task> aria2_tasks = Methods.get_tasks(ARIA2_HOST, ARIA2_PORT, ARIA2_JSON_RPC_FILE_NAME, ARIA2_ACCESS_PROTOCOL);

            foreach(Aria2_Task aria2_task in aria2_tasks)
            {
                Log_Utils.Add_system_event_and_log(Aria2_Client_Constants.EVENT_SOURCE, "Task URL : " + aria2_task.files[0].uris[0].uri, EventLogEntryType.Information);
            }
            
            //var get_response = Network_Utils.Get_Request(Network_Utils.Get_parametered_URL(API_Wrapper.get_API(API_Constants.SELECT_TASKS), new List<KeyValuePair<string, string>>
            //{
            //    new KeyValuePair<string, string>("host", Environment.MachineName)
            //}));

            //Log_Utils.Add_system_event_and_log(Aria2_Service_Constants.event_source, "Existing Tasks : " + get_response, EventLogEntryType.Information);

            //JArray array = JArray.Parse(get_response);
            //if ((Int32)JObject.Parse(array[0].ToString())["error_status"] == 1)
            //{
            //    Log_Utils.Add_system_event_and_log("aria2c_rpc", "Error : " + JObject.Parse(array[0].ToString())["error"] + " - " + JObject.Parse(array[0].ToString())["error_number"], EventLogEntryType.Information);
            //}
            //else if ((Int32)JObject.Parse(array[0].ToString())["error_status"] == 2)
            //{
            //    Log_Utils.Add_system_event_and_log("aria2c_rpc", "No Tasks", EventLogEntryType.Information);
            //}
            //else if ((Int32)JObject.Parse(array[0].ToString())["error_status"] == 0)
            //{
            //    List<Task> tasks = new List<Task>();
            //    for (int i = 1; i < array.Count; i++)
            //    {
            //        JObject json_Task = JObject.Parse(array[i].ToString());
            //        Task current_task = new Task
            //        {
            //            id = (String)json_Task["id"],
            //            gid = (String)json_Task["gid"]
            //        };
            //        tasks.Add(current_task);

            //        Log_Utils.Add_system_event_and_log("aria2c_rpc", "ID : " + current_task.id + ", gid : " + current_task.gid, EventLogEntryType.Information);
            //        Log_Utils.Add_system_event_and_log("aria2c_rpc", "Request : " + Create_json_request_tellStatus(current_task.gid, current_task.id), EventLogEntryType.Information);

            //        try
            //        {
            //            var status_response = webClient.UploadString("http://localhost:6800/jsonrpc", "POST", Create_json_request_tellStatus(current_task.gid, current_task.id));
            //            Log_Utils.Add_system_event_and_log("aria2c_rpc", "Task tellStatus Response : " + status_response, EventLogEntryType.Information);

            //            JObject json_object = JObject.Parse(status_response);

            //            Update_task_current_status(current_task.id, json_object.ToString());
            //        }
            //        catch (Exception e)
            //        {
            //            Log_Utils.Add_system_event_and_log("aria2c_rpc", "Task exception : " + e.ToString(), EventLogEntryType.Information);
            //        }
            //    }
            //}
            //else
            //{
            //    Log_Utils.Add_system_event_and_log("aria2c_rpc", "Check response, response : " + get_response, EventLogEntryType.Information);
            //}
        }

        public static void Get_Tasks()
        {
            //Console.WriteLine(Environment.MachineName + " Sync. Started...");
            //Console.WriteLine(API_Wrapper.get_API(API_Wrapper.SELECT_TASKS));
            //var get_response = http_client.Get(API_Wrapper.get_API(API_Wrapper.SELECT_TASKS), new { host = Environment.MachineName });
            //Console.WriteLine("New Tasks : " + get_response.RawText);
            //Console.ReadKey();

            Log_Utils.Add_system_event_and_log(Aria2_Client_Constants.EVENT_SOURCE, Environment.MachineName + " Sync. Started...", EventLogEntryType.Information);
            var response = Network_Utils.Get_Request(Network_Utils.Get_parametered_URL(API_Wrapper.get_API(Aria2_Remote_API_Constants.SELECT_TASKS),new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("host",Environment.MachineName)}));
            Log_Utils.Add_system_event_and_log(Aria2_Client_Constants.EVENT_SOURCE, "New Tasks : " + response.ToString(), EventLogEntryType.Information);

            //TODO : Add No Entry on Error Check

            JArray array = JArray.Parse(response);

            if ((Int32)JObject.Parse(array[0].ToString())["error_status"] == 1)
            {
                Log_Utils.Add_system_event_and_log(Aria2_Client_Constants.EVENT_SOURCE, "Error : " + array[0]["error"] + " - " + array[0]["error_number"], EventLogEntryType.Information);
            }
            else if ((Int32)JObject.Parse(array[0].ToString())["error_status"] == 2)
            {
                Log_Utils.Add_system_event_and_log(Aria2_Client_Constants.EVENT_SOURCE, "No Tasks", EventLogEntryType.Information); Console.WriteLine("");
            }
            else if ((Int32)JObject.Parse(array[0].ToString())["error_status"] == 0)
            {
                //List<Task> tasks = new List<Task>();
                //for (int i = 1; i < array.Count; i++)
                //{
                //    JObject json_Task = JObject.Parse(array[i].ToString());
                //    Task current_task = new Task();
                //    current_task.id = (String)json_Task["id"];
                //    current_task.url = (String)json_Task["url"];
                //    tasks.Add(current_task);

                //    Console.WriteLine("ID : " + current_task.id + ", Task : " + current_task.url);
                //    Console.WriteLine("Request : " + Create_json_request_addUri(current_task.url, current_task.id));
                //    //Console.ReadKey();

                //    var add_response = webClient.UploadString("http://localhost:6800/jsonrpc", "POST", Create_json_request_addUri(current_task.url, current_task.id));
                //    Console.WriteLine("Task Addition Response : " + add_response);
                //    //Console.ReadKey();

                //    JObject json_object = JObject.Parse(add_response);

                //    Update_task_gid(current_task.id, json_object["result"].ToString());
                //}
            }
            else
            {
                Log_Utils.Add_system_event_and_log(Aria2_Client_Constants.EVENT_SOURCE, "Check response, response : " + response, EventLogEntryType.Information);
            }
        }

        private static void Update_task_gid(String id, String gid)
        {

            //var client = new HttpClient();

            //var pairs = new List<KeyValuePair<string, string>>
            //{
            //    new KeyValuePair<string, string>("id", id),
            //    new KeyValuePair<string, string>("gid", gid)
            //};

            //var content = new FormUrlEncodedContent(pairs);

            //var update_response = client.PostAsync(API_Wrapper.get_API(API_Wrapper.UPDATE_TASK_GID), content).Result;

            //if (update_response.IsSuccessStatusCode)
            //{
            //    Console.WriteLine(update_response.Content.ReadAsStringAsync().Result);
            //    //Console.ReadKey();

            //    if ((Int32)JObject.Parse(update_response.Content.ReadAsStringAsync().Result)["error_status"] == 0)
            //    {
            //        Console.WriteLine("Task updated successfully");
            //    }
            //    else if ((Int32)JObject.Parse(update_response.Content.ReadAsStringAsync().Result)["error_status"] == 1)
            //    {
            //        Console.WriteLine("Error : " + JObject.Parse(update_response.Content.ReadAsStringAsync().Result)["error"] + " - " + JObject.Parse(update_response.Content.ReadAsStringAsync().Result)["error_number"]);
            //    }
            //    else
            //    {
            //        Console.WriteLine("Check response");
            //    }

            //}
        }

        private static void Update_task_current_status(String id, String current_status)
        {

            //var client = new HttpClient();

            //var pairs = new List<KeyValuePair<string, string>>
            //{
            //    new KeyValuePair<string, string>("id", id),
            //    new KeyValuePair<string, string>("current_status", current_status)
            //};

            //var content = new FormUrlEncodedContent(pairs);

            //var update_response = client.PostAsync(API_Wrapper.get_API(API_Wrapper.UPDATE_TASK_CURRENT_STATUS), content).Result;

            //if (update_response.IsSuccessStatusCode)
            //{
            //    Console.WriteLine(update_response.Content.ReadAsStringAsync().Result);
            //    //Console.ReadKey();

            //    if ((Int32)JObject.Parse(update_response.Content.ReadAsStringAsync().Result)["error_status"] == 0)
            //    {
            //        Console.WriteLine("Task updated successfully");
            //    }
            //    else if ((Int32)JObject.Parse(update_response.Content.ReadAsStringAsync().Result)["error_status"] == 1)
            //    {
            //        Console.WriteLine("Error : " + JObject.Parse(update_response.Content.ReadAsStringAsync().Result)["error"] + " - " + JObject.Parse(update_response.Content.ReadAsStringAsync().Result)["error_number"]);
            //    }
            //    else
            //    {
            //        Console.WriteLine("Check response, response : " + update_response);
            //    }

            //}
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

    }
}

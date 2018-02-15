using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net.Http;

namespace aria2c_service
{
    public partial class aria2c_remote : ServiceBase
    {
        int aria2c_process_id;
        static EasyHttp.Http.HttpClient http_client = new EasyHttp.Http.HttpClient();
        static WebClient webClient = new WebClient();
        public aria2c_remote()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;

            startInfo.FileName = @"C:\Programs\aria2-1.33.1-win-64bit-build1\aria2c.exe";
            string arguments = @"--conf-path C:\Programs\aria2_repository\aria2.conf --log=C:\Programs\aria2_repository\aria2_rpc.log";

            //startInfo.FileName = @"F:\Programs\aria2-1.33.1-win-64bit-build1\aria2c.exe";
            //string arguments = @"--conf-path F:\Programs\aria2.conf --log=F:\Programs\aria2_rpc.log";

            write_event_logs_for_application("aria2c_rpc", "arguments " + arguments, EventLogEntryType.Information);
            startInfo.Arguments = arguments;
            
            aria2c_process_id = Process.Start(startInfo).Id;

            while (true)
            {
                aria2c_service_main();
                Thread.Sleep(1 * 60 * 1000);
            }
        }

        public static void write_event_logs_for_application(string sSource, string event_message, EventLogEntryType event_type)
        {
            if (!EventLog.SourceExists(sSource))
                EventLog.CreateEventSource(sSource, "Application");

            EventLog.WriteEntry(sSource, event_message, event_type);
        }

        protected override void OnStop()
        {

            try
            {
                Process proc = Process.GetProcessById(aria2c_process_id);
                proc.Kill();
            }
            catch (Exception ex)
            {

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
            //Console.WriteLine(Environment.MachineName + " Sync. Started...");
            write_event_logs_for_application("aria2c_rpc", Environment.MachineName + " Sync. Started...", EventLogEntryType.Information);
            var get_response = http_client.Get(API.get_API(API.select_Tasks), new { host = Environment.MachineName });
            //Console.WriteLine("New Tasks : " + get_response.RawText);
            write_event_logs_for_application("aria2c_rpc", "New Tasks : " + get_response.RawText, EventLogEntryType.Information);
            //Console.ReadKey();


            JArray array = JArray.Parse(get_response.RawText);
            if ((Int32)JObject.Parse(array[0].ToString())["error_status"] == 1)
            {
                //Console.WriteLine("Error : " + JObject.Parse(array[0].ToString())["error"] + " - " + JObject.Parse(array[0].ToString())["error_number"]);
                write_event_logs_for_application("aria2c_rpc", "Error : " + JObject.Parse(array[0].ToString())["error"] + " - " + JObject.Parse(array[0].ToString())["error_number"], EventLogEntryType.Information);

            }
            else if ((Int32)JObject.Parse(array[0].ToString())["error_status"] == 2)
            {
                //Console.WriteLine("No Tasks");
                write_event_logs_for_application("aria2c_rpc", "No Tasks", EventLogEntryType.Information);

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

                    //Console.WriteLine("ID : " + current_task.id + ", Task : " + current_task.url);
                    write_event_logs_for_application("aria2c_rpc", "ID : " + current_task.id + ", Task : " + current_task.url, EventLogEntryType.Information);

                    //Console.WriteLine("Request : " + create_json_request(current_task.url, current_task.id));
                    write_event_logs_for_application("aria2c_rpc", "Request : " + create_json_request(current_task.url, current_task.id), EventLogEntryType.Information);

                    //Console.ReadKey();

                    var add_response = webClient.UploadString("http://localhost:6800/jsonrpc", "POST", create_json_request(current_task.url, current_task.id));
                    //Console.WriteLine("Task Addition Response : " + add_response);
                    write_event_logs_for_application("aria2c_rpc", "Task Addition Response : " + add_response, EventLogEntryType.Information);

                    //Console.ReadKey();

                    JObject json_object = JObject.Parse(add_response);

                    update_task(current_task.id, json_object["result"].ToString());

                }

            }
            else
            {
                //Console.WriteLine("Check response");
                write_event_logs_for_application("aria2c_rpc", "Check response", EventLogEntryType.Information);

            }
        }

        private static void update_task(String id, String gid)
        {
            var client = new HttpClient();

            var pairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("id", id),
                new KeyValuePair<string, string>("gid", gid)
            };

            var content = new FormUrlEncodedContent(pairs);

            var update_response = client.PostAsync(API.get_API(API.update_Task), content).Result;

            if (update_response.IsSuccessStatusCode)
            {
                //Console.WriteLine(update_response.Content.ReadAsStringAsync().Result);
                write_event_logs_for_application("aria2c_rpc","Update task response : "+ update_response.Content.ReadAsStringAsync().Result, EventLogEntryType.Information);

                //Console.ReadKey();

                if ((Int32)JObject.Parse(update_response.Content.ReadAsStringAsync().Result)["error_status"] == 0)
                {
                    //Console.WriteLine("Task updated successfully");
                    write_event_logs_for_application("aria2c_rpc", "Task updated successfully", EventLogEntryType.Information);

                }
                else if ((Int32)JObject.Parse(update_response.Content.ReadAsStringAsync().Result)["error_status"] == 1)
                {
                    //Console.WriteLine("Error : " + JObject.Parse(update_response.Content.ReadAsStringAsync().Result)["error"] + " - " + JObject.Parse(update_response.Content.ReadAsStringAsync().Result)["error_number"]);
                    write_event_logs_for_application("aria2c_rpc", "Error : " + JObject.Parse(update_response.Content.ReadAsStringAsync().Result)["error"] + " - " + JObject.Parse(update_response.Content.ReadAsStringAsync().Result)["error_number"], EventLogEntryType.Information);

                }
                else
                {
                    //Console.WriteLine("Check response");
                    write_event_logs_for_application("aria2c_rpc", "Check response", EventLogEntryType.Information);

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
                    //Console.WriteLine("System status is OK");
                    write_event_logs_for_application("aria2c_rpc", "System status is OK", EventLogEntryType.Information);

                    update_host();
                    return true;
                }
                else if ((Int32)JObject.Parse(array[1].ToString())["system_status"] == 0)
                {
                    //Console.WriteLine("System is in maintanace mode");
                    write_event_logs_for_application("aria2c_rpc", "System is in maintanace mode", EventLogEntryType.Information);

                }
                else
                {
                    //Console.WriteLine("Check response");
                    write_event_logs_for_application("aria2c_rpc", "Check response", EventLogEntryType.Information);

                }

            }
            else if ((Int32)JObject.Parse(array[0].ToString())["error_status"] == 0)
            {
                //Console.WriteLine("Error : " + JObject.Parse(array[0].ToString())["error"] + " - " + JObject.Parse(array[0].ToString())["error_number"]);
                write_event_logs_for_application("aria2c_rpc", "Error : " + JObject.Parse(array[0].ToString())["error"] + " - " + JObject.Parse(array[0].ToString())["error_number"], EventLogEntryType.Information);

            }
            else
            {
                //Console.WriteLine("Check response");
                write_event_logs_for_application("aria2c_rpc", "Check response", EventLogEntryType.Information);

            }
            return false;
        }

        private static void update_host()
        {
            
            var client = new HttpClient();

            var pairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("name", Environment.MachineName)
            };

            var content = new FormUrlEncodedContent(pairs);

            var update_response = client.PostAsync(API.get_API(API.update_Host), content).Result;

            if (update_response.IsSuccessStatusCode)
            {
                //Console.WriteLine(update_response.Content.ReadAsStringAsync().Result);
                write_event_logs_for_application("aria2c_rpc","Host update response : "+ update_response.Content.ReadAsStringAsync().Result, EventLogEntryType.Information);
                //Console.ReadKey();

                if ((Int32)JObject.Parse(update_response.Content.ReadAsStringAsync().Result)["error_status"] == 0)
                {
                    //Console.WriteLine("Host updated successfully");
                    write_event_logs_for_application("aria2c_rpc", "Host updated successfully", EventLogEntryType.Information);

                }
                else if ((Int32)JObject.Parse(update_response.Content.ReadAsStringAsync().Result)["error_status"] == 1)
                {
                    //Console.WriteLine("Error : " + JObject.Parse(update_response.Content.ReadAsStringAsync().Result)["error"] + " - " + JObject.Parse(update_response.Content.ReadAsStringAsync().Result)["error_number"]);
                    write_event_logs_for_application("aria2c_rpc", "Error : " + JObject.Parse(update_response.Content.ReadAsStringAsync().Result)["error"] + " - " + JObject.Parse(update_response.Content.ReadAsStringAsync().Result)["error_number"], EventLogEntryType.Information);

                }
                else
                {
                    //Console.WriteLine("Check response");
                    write_event_logs_for_application("aria2c_rpc", "Check response", EventLogEntryType.Information);

                }

            }


        }

    }
}


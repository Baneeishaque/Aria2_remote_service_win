using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Timers;
using System.IO;
using System.Text;
using System.Configuration;

namespace aria2c_service
{
    public partial class aria2c_remote : ServiceBase
    {
        int aria2c_process_id;
        static WebClient webClient = new WebClient();

        /// <summary>
        /// This timer willl run the process at the interval specified (currently 1 minute) once enabled
        /// </summary>
        Timer timer = new System.Timers.Timer(1000 * 60);

        public aria2c_remote()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;

            //startInfo.FileName = @"C:\Programs\aria2-1.33.1-win-64bit-build1\aria2c.exe";
            //string arguments = @"--conf-path C:\Programs\aria2_repository\aria2.conf --log=C:\Programs\aria2_repository\aria2_rpc.log";

            startInfo.FileName = @ConfigurationManager.AppSettings["aria2c_HOME"] + "\\aria2c.exe";
            string arguments = @"--conf-path " + ConfigurationManager.AppSettings["aria2c_repository"] + "\\aria2.conf --log=" + ConfigurationManager.AppSettings["aria2c_repository"] + "\\aria2_rpc.log";

            Write_event_logs_for_application("aria2c_rpc", "arguments " + arguments, EventLogEntryType.Information);
            startInfo.Arguments = arguments;

            aria2c_process_id = Process.Start(startInfo).Id;

            // point the timer elapsed to the handler
            timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
            // turn on the timer
            timer.Enabled = true;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Aria2c_service_main();
        }

        public static void Write_event_logs_for_application(string sSource, string event_message, EventLogEntryType event_type)
        {
            if (!EventLog.SourceExists(sSource))
                EventLog.CreateEventSource(sSource, "Application");

            EventLog.WriteEntry(sSource, event_message, event_type);
        }

        protected override void OnStop()
        {
            timer.Enabled = false;
            try
            {
                Process proc = Process.GetProcessById(aria2c_process_id);
                proc.Kill();
            }
            catch (Exception ex)
            {
                Write_event_logs_for_application("aria2c_rpc", "Kill Exception " + ex.ToString(), EventLogEntryType.Error);
            }
        }

        private static int secondsElapsed = 0;
        public static void Aria2c_service_main()
        {
            secondsElapsed += 10;
            if (Check_system_status())
            {
                Get_Tasks();
            }
        }

        public static string Get(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static string Get_parametered_URL(string base_URL, List<KeyValuePair<string, string>> openWith)
        {
            for (int i = 0; i < openWith.Count; i++)
            {
                KeyValuePair<string, string> kvp = openWith[i];
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

        public static void Get_Tasks()
        {
            Write_event_logs_for_application("aria2c_rpc", Environment.MachineName + " Sync. Started...", EventLogEntryType.Information);

            var get_response = Get(Get_parametered_URL(API.get_API(API.select_Tasks), new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("host", Environment.MachineName)
            }));

            Write_event_logs_for_application("aria2c_rpc", "New Tasks : " + get_response, EventLogEntryType.Information);

            JArray array = JArray.Parse(get_response);
            if ((Int32)JObject.Parse(array[0].ToString())["error_status"] == 1)
            {
                Write_event_logs_for_application("aria2c_rpc", "Error : " + JObject.Parse(array[0].ToString())["error"] + " - " + JObject.Parse(array[0].ToString())["error_number"], EventLogEntryType.Information);
            }
            else if ((Int32)JObject.Parse(array[0].ToString())["error_status"] == 2)
            {
                Write_event_logs_for_application("aria2c_rpc", "No Tasks", EventLogEntryType.Information);
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
                        url = ((String)json_Task["url"]).Contains("http") ? (String)json_Task["url"] : "http://" + (String)json_Task["url"]
                    };
                    tasks.Add(current_task);

                    Write_event_logs_for_application("aria2c_rpc", "ID : " + current_task.id + ", Task : " + current_task.url, EventLogEntryType.Information);

                    Write_event_logs_for_application("aria2c_rpc", "Request : " + Create_json_request_addUri(current_task.url, current_task.id), EventLogEntryType.Information);

                    var add_response = webClient.UploadString("http://localhost:6800/jsonrpc", "POST", Create_json_request_addUri(current_task.url, current_task.id));
                    Write_event_logs_for_application("aria2c_rpc", "Task Addition Response : " + add_response, EventLogEntryType.Information);

                    JObject json_object = JObject.Parse(add_response);

                    Update_task(current_task.id, json_object["result"].ToString());
                }

            }
            else
            {
                Write_event_logs_for_application("aria2c_rpc", "Check response, response : " + get_response, EventLogEntryType.Information);
            }
        }

        private static void Update_task(String id, String gid)
        {
            var request = (HttpWebRequest)WebRequest.Create(API.get_API(API.update_Task_gid));

            var postData = "id=" + id;
            postData += "&gid=" + gid;

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

                Write_event_logs_for_application("aria2c_rpc", "Update task response : " + responseString, EventLogEntryType.Information);

                if ((Int32)JObject.Parse(responseString)["error_status"] == 0)
                {
                    Write_event_logs_for_application("aria2c_rpc", "Task updated successfully", EventLogEntryType.Information);
                }
                else if ((Int32)JObject.Parse(responseString)["error_status"] == 1)
                {
                    Write_event_logs_for_application("aria2c_rpc", "Error : " + JObject.Parse(responseString)["error"] + " - " + JObject.Parse(responseString)["error_number"], EventLogEntryType.Information);
                }
                else
                {
                    Write_event_logs_for_application("aria2c_rpc", "Check response, response : " + response, EventLogEntryType.Information);
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

        private static bool Check_system_status()
        {
            var response = Get(API.get_API(API.select_Configuration));

            JArray array = JArray.Parse(response);

            if ((Int32)JObject.Parse(array[0].ToString())["error_status"] == 0)
            {
                if ((Int32)JObject.Parse(array[1].ToString())["system_status"] == 1)
                {
                    Write_event_logs_for_application("aria2c_rpc", "System status is OK", EventLogEntryType.Information);
                    Update_host();
                    Update_tasks();
                    return true;
                }
                else if ((Int32)JObject.Parse(array[1].ToString())["system_status"] == 0)
                {
                    Write_event_logs_for_application("aria2c_rpc", "System is in maintanace mode", EventLogEntryType.Information);
                }
                else
                {
                    Write_event_logs_for_application("aria2c_rpc", "Check response, response : " + response, EventLogEntryType.Information);
                }
            }
            else if ((Int32)JObject.Parse(array[0].ToString())["error_status"] == 0)
            {
                Write_event_logs_for_application("aria2c_rpc", "Error : " + JObject.Parse(array[0].ToString())["error"] + " - " + JObject.Parse(array[0].ToString())["error_number"], EventLogEntryType.Information);
            }
            else
            {
                Write_event_logs_for_application("aria2c_rpc", "Check response, response : " + response, EventLogEntryType.Information);
            }
            return false;
        }

        private static void Update_tasks()
        {
            Write_event_logs_for_application("aria2c_rpc", Environment.MachineName + " Sync. Started...", EventLogEntryType.Information);

            var get_response = Get(Get_parametered_URL(API.get_API(API.select_Tasks), new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("host", Environment.MachineName)
            }));

            Write_event_logs_for_application("aria2c_rpc", "Existing Tasks : " + get_response, EventLogEntryType.Information);

            JArray array = JArray.Parse(get_response);
            if ((Int32)JObject.Parse(array[0].ToString())["error_status"] == 1)
            {
                Write_event_logs_for_application("aria2c_rpc", "Error : " + JObject.Parse(array[0].ToString())["error"] + " - " + JObject.Parse(array[0].ToString())["error_number"], EventLogEntryType.Information);
            }
            else if ((Int32)JObject.Parse(array[0].ToString())["error_status"] == 2)
            {
                Write_event_logs_for_application("aria2c_rpc", "No Tasks", EventLogEntryType.Information);
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

                    Write_event_logs_for_application("aria2c_rpc", "ID : " + current_task.id + ", gid : " + current_task.gid, EventLogEntryType.Information);
                    Write_event_logs_for_application("aria2c_rpc", "Request : " + Create_json_request_tellStatus(current_task.gid, current_task.id), EventLogEntryType.Information);

                    try
                    {
                        var status_response = webClient.UploadString("http://localhost:6800/jsonrpc", "POST", Create_json_request_tellStatus(current_task.gid, current_task.id));
                        Write_event_logs_for_application("aria2c_rpc", "Task tellStatus Response : " + status_response, EventLogEntryType.Information);

                        JObject json_object = JObject.Parse(status_response);

                        Update_task_current_status(current_task.id, json_object.ToString());
                    }
                    catch (Exception e)
                    {
                        Write_event_logs_for_application("aria2c_rpc", "Task exception : " + e.ToString(), EventLogEntryType.Information);
                    }
                }
            }
            else
            {
                Write_event_logs_for_application("aria2c_rpc", "Check response, response : " + get_response, EventLogEntryType.Information);
            }
        }

        private static void Update_task_current_status(String id, String current_status)
        {
            var request = (HttpWebRequest)WebRequest.Create(API.get_API(API.update_Task_current_status));

            var postData = "id=" + id;
            postData += "&current_status=" + current_status;
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
                //write_event_logs_for_application("aria2c_rpc", "Host update response : " + responseString, EventLogEntryType.Information);

                if ((Int32)JObject.Parse(responseString)["error_status"] == 0)
                {
                    Write_event_logs_for_application("aria2c_rpc", "Task updated successfully", EventLogEntryType.Information);
                }
                else if ((Int32)JObject.Parse(responseString)["error_status"] == 1)
                {
                    Write_event_logs_for_application("aria2c_rpc", "Error : " + JObject.Parse(responseString)["error"] + " - " + JObject.Parse(responseString)["error_number"], EventLogEntryType.Information);
                }
                else
                {
                    Write_event_logs_for_application("aria2c_rpc", "Check response, response : " + response, EventLogEntryType.Information);
                }
            }
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
                //write_event_logs_for_application("aria2c_rpc", "Host update response : " + responseString, EventLogEntryType.Information);

                if ((Int32)JObject.Parse(responseString)["error_status"] == 0)
                {
                    Write_event_logs_for_application("aria2c_rpc", "Host updated successfully", EventLogEntryType.Information);
                }
                else if ((Int32)JObject.Parse(responseString)["error_status"] == 1)
                {
                    Write_event_logs_for_application("aria2c_rpc", "Error : " + JObject.Parse(responseString)["error"] + " - " + JObject.Parse(responseString)["error_number"], EventLogEntryType.Information);
                }
                else
                {
                    Write_event_logs_for_application("aria2c_rpc", "Check response, response : " + response, EventLogEntryType.Information);
                }
            }
        }
    }
}


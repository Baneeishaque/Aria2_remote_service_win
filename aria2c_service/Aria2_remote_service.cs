using System.ServiceProcess;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using commons_server_client_lib;
using aria2_client_lib;
using aria2_common_lib;
using Aria2_Remote_Common_Lib;

namespace Aria2_Remote_Service
{
    public partial class Aria2_remote_service : ServiceBase
    {
        int aria2c_process_id;

        static WebClient webClient = new WebClient();
        private readonly IService_Client_Utils aria2_client_interface = new Aria2_Client(API_Wrapper.get_API(Aria2_Remote_API_Constants.SELECT_CONFIGURATION), API_Wrapper.get_API(Aria2_Remote_API_Constants.UPDATE_HOST), Aria2_RPC_Constants.HOST);

        public Aria2_remote_service()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            aria2c_process_id = aria2_client_interface.Start_client();
        }

        protected override void OnStop()
        {
            aria2_client_interface.Stop_client(aria2c_process_id);
        }

        private static int secondsElapsed = 0;



        //public void Aria2_remote()
        //{
        //    secondsElapsed += 10;

        //    //TODO : Check System Updation

        //    int server_status = Server_Utils.Check_system_status(Aria2_Service_Constants.event_source, API_Wrapper.get_API(API_Constants.SELECT_CONFIGURATION));

        //    if (server_status == 1)
        //    {
        //        Update_host();
        //        Update_tasks();
        //        //Get_Tasks();
        //    }
        //    else if (server_status == -1)
        //    {
        //        Stop_client();
        //    }
        //}

        //public static void Get_Tasks()
        //{
        //    Log_Utils.Add_system_event_and_log("aria2c_rpc", Environment.MachineName + " Sync. Started...", EventLogEntryType.Information);

        //    var get_response = Network_Utils.Get_Request(Network_Utils.Get_parametered_URL(API_Wrapper.get_API(API_Constants.SELECT_TASKS), new List<KeyValuePair<string, string>>
        //    {
        //        new KeyValuePair<string, string>("host", Environment.MachineName)
        //    }));

        //    Log_Utils.Add_system_event_and_log("aria2c_rpc", "New Tasks : " + get_response, EventLogEntryType.Information);

        //    JArray array = JArray.Parse(get_response);
        //    if ((Int32)JObject.Parse(array[0].ToString())["error_status"] == 1)
        //    {
        //        Log_Utils.Add_system_event_and_log("aria2c_rpc", "Error : " + JObject.Parse(array[0].ToString())["error"] + " - " + JObject.Parse(array[0].ToString())["error_number"], EventLogEntryType.Information);
        //    }
        //    else if ((Int32)JObject.Parse(array[0].ToString())["error_status"] == 2)
        //    {
        //        Log_Utils.Add_system_event_and_log("aria2c_rpc", "No Tasks", EventLogEntryType.Information);
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
        //                url = (String)json_Task["url"]
        //            };
        //            tasks.Add(current_task);

        //            Log_Utils.Add_system_event_and_log("aria2c_rpc", "ID : " + current_task.id + ", Task : " + current_task.url, EventLogEntryType.Information);

        //            Log_Utils.Add_system_event_and_log("aria2c_rpc", "Request : " + Create_json_request_addUri(current_task.url, current_task.id), EventLogEntryType.Information);

        //            var add_response = webClient.UploadString("http://localhost:6800/jsonrpc", "POST", Create_json_request_addUri(current_task.url, current_task.id));
        //            Log_Utils.Add_system_event_and_log("aria2c_rpc", "Task Addition Response : " + add_response, EventLogEntryType.Information);

        //            JObject json_object = JObject.Parse(add_response);

        //            Update_task(current_task.id, json_object["result"].ToString());
        //        }

        //    }
        //    else
        //    {
        //        Log_Utils.Add_system_event_and_log("aria2c_rpc", "Check response, response : " + get_response, EventLogEntryType.Information);
        //    }
        //}

        //private static void Update_task(String id, String gid)
        //{
        //    var request = (HttpWebRequest)WebRequest.Create(API_Wrapper.get_API(API_Constants.UPDATE_TASK_GID));

        //    var postData = "id=" + id;
        //    postData += "&gid=" + gid;

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

        //        Log_Utils.Add_system_event_and_log("aria2c_rpc", "Update task response : " + responseString, EventLogEntryType.Information);

        //        if ((Int32)JObject.Parse(responseString)["error_status"] == 0)
        //        {
        //            Log_Utils.Add_system_event_and_log("aria2c_rpc", "Task updated successfully", EventLogEntryType.Information);
        //        }
        //        else if ((Int32)JObject.Parse(responseString)["error_status"] == 1)
        //        {
        //            Log_Utils.Add_system_event_and_log("aria2c_rpc", "Error : " + JObject.Parse(responseString)["error"] + " - " + JObject.Parse(responseString)["error_number"], EventLogEntryType.Information);
        //        }
        //        else
        //        {
        //            Log_Utils.Add_system_event_and_log("aria2c_rpc", "Check response, response : " + response, EventLogEntryType.Information);
        //        }
        //    }
        //}

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



        //private static void Update_tasks()
        //{
        //    Log_Utils.Add_system_event_and_log(Aria2_Service_Constants.event_source, Environment.MachineName + " Updating task status...", EventLogEntryType.Information);

        //    TellAll.get_tasks(Aria2_Service_Constants.host);

        //            Log_Utils.Add_system_event_and_log("aria2c_rpc", "Task tellStatus Response : " + status_response, EventLogEntryType.Information);


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
        //}

        //private static void Update_task_current_status(String id, String current_status)
        //{
        //    var request = (HttpWebRequest)WebRequest.Create(API_Wrapper.get_API(API_Constants.UPDATE_TASK_CURRENT_STATUS));

        //    var postData = "id=" + id;
        //    postData += "&current_status=" + current_status;
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
        //        //Log_Utils.Add_system_event_and_log("aria2c_rpc", "Host update response : " + responseString, EventLogEntryType.Information);

        //        if ((Int32)JObject.Parse(responseString)["error_status"] == 0)
        //        {
        //            Log_Utils.Add_system_event_and_log("aria2c_rpc", "Task updated successfully", EventLogEntryType.Information);
        //        }
        //        else if ((Int32)JObject.Parse(responseString)["error_status"] == 1)
        //        {
        //            Log_Utils.Add_system_event_and_log("aria2c_rpc", "Error : " + JObject.Parse(responseString)["error"] + " - " + JObject.Parse(responseString)["error_number"], EventLogEntryType.Information);
        //        }
        //        else
        //        {
        //            Log_Utils.Add_system_event_and_log("aria2c_rpc", "Check response, response : " + response, EventLogEntryType.Information);
        //        }
        //    }
        //}

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

        //private void Update_host()
        //{
        //    //TODO : Update Drive free spaces

        //    Dictionary<string, string> data_items = new Dictionary<string, string>() { { "name", Environment.MachineName } };

        //    var response = Network_Utils.Post_Request(API_Wrapper.get_API(API_Constants.UPDATE_HOST), data_items);

        //    if (response.StatusCode == HttpStatusCode.OK)
        //    {
        //        var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
        //        //Log_Utils.Add_system_event_and_log("aria2c_rpc", "Host update response : " + responseString, EventLogEntryType.Information);

        //        if ((Int32)JObject.Parse(responseString)["error_status"] == 0)
        //        {
        //            Log_Utils.Add_system_event_and_log(Aria2_Service_Constants.event_source, "Host updated successfully", EventLogEntryType.Information);
        //        }
        //        else if ((Int32)JObject.Parse(responseString)["error_status"] == 1)
        //        {
        //            Log_Utils.Add_system_event_and_log(Aria2_Service_Constants.event_source, "Error : " + JObject.Parse(responseString)["error"] + " - " + JObject.Parse(responseString)["error_number"], EventLogEntryType.Information);
        //        }
        //        else
        //        {
        //            Log_Utils.Add_system_event_and_log(Aria2_Service_Constants.event_source, "Check response, response : " + response, EventLogEntryType.Information);
        //        }
        //    }
        //}

    }
}


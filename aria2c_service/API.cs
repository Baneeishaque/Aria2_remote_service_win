using System;

namespace aria2c_service
{
    class API
    {
        public static String API_Root = "http_API";

        public static String select_Configuration = "select_Configuration.php";
        public static String select_Tasks = "select_Tasks.php";
        public static String update_Task = "update_Task.php";
        public static String update_Host = "update_Host.php";

        public static String get_API(String API_Method)
        {
            return Server_Endpiont.Server_IP_Address+"/"+Server_Endpiont.Server_Application_Root + "/" + API_Root+"/"+API_Method;
        }
    }
}

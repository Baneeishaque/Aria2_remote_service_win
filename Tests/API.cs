using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    class API
    {
        public static String API_Root = "http_API";

        //public static String select_Members = "select_Members.php";
        //public static String select_Departments = "select_Departments.php";
        //public static String select_Semesters = "select_Semesters.php";
        //public static String insert_Member = "insert_Member.php";
        //public static String insert_Transaction = "insert_Transaction.php";
        public static String select_Configuration = "select_Configuration.php";
        public static String select_Tasks = "select_Tasks.php";

        public static String get_API(String API_Method)
        {
            return Server_Endpiont.Server_IP_Address+"/"+Server_Endpiont.Server_Application_Root + "/" + API_Root+"/"+API_Method;
        }
    }
}

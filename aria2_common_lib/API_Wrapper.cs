using commons_lib;
using System;

namespace aria2_common
{
    public class API_Wrapper : Http_API_Constants
    {
        public static String get_API(String API_METHOD)
        {
            return API_Utils.get_API(Aria2_Remote_Server_Endpiont.SERVER_IP_ADDRESS, Aria2_Remote_Server_Endpiont.SERVER_APPLICATION_ROOT, API_ROOT, API_METHOD, Aria2_Remote_Server_Endpiont.FILE_EXTENSION);
        }
    }
}

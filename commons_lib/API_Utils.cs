using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace commons_lib
{
    public class API_Utils
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static String get_API(String SERVER_IP_ADDRESS,String SERVER_APPLICATION_ROOT,String API_ROOT,String API_METHOD,String FILE_EXTENSION)
        {
            log.Info("API URL : " + SERVER_IP_ADDRESS + "/" + SERVER_APPLICATION_ROOT + "/" + API_ROOT + "/" + API_METHOD + "." + FILE_EXTENSION);

            return SERVER_IP_ADDRESS + "/" + SERVER_APPLICATION_ROOT + "/" + API_ROOT + "/" + API_METHOD + "." + FILE_EXTENSION;
        }
    }
}

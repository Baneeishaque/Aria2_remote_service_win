using commons_server_client_lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using commons_lib;

namespace aria2_client_lib
{
    public abstract class Aria2_Client_Wrapper : IService_Client_Utils
    {
        public abstract int Start_Aria2();

        public int Start_client()
        {
            return Start_Aria2();
        }

        public abstract bool Stop_Aria2(int aria2_process_id);

        public bool Stop_client(int client_process_id)
        {
            return Stop_Aria2(client_process_id);
        }

    }
}

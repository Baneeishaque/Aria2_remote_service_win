using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace commons_lib
{
    public interface IClient_Utils
    {
        bool Stop_client(int client_process_id);

        int Start_client();
    }
}

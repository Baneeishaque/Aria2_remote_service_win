using aria2_client_lib;
using aria2_common_lib;
using Aria2_Remote_Common_Lib;
using commons_server_client_lib;
using System;
using System.Net;

namespace Aria2_Client_Program
{
    class Program : IService_Client_Utils
    {

        private readonly IService_Client_Utils aria2_client_interface = new Aria2_Client(API_Wrapper.get_API(Aria2_Remote_API_Constants.SELECT_CONFIGURATION), API_Wrapper.get_API(Aria2_Remote_API_Constants.UPDATE_HOST), Aria2_RPC_Constants.HOST);

        static void Main(string[] args)
        {
            Program p = new Program();
            p.Start_client();

            Console.ReadKey();
        }

        public int Start_client()
        {
            return aria2_client_interface.Start_client();
        }

        public bool Stop_client(int client_process_id)
        {
            return aria2_client_interface.Stop_client(client_process_id);
        }

    }
}


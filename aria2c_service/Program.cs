using System.ServiceProcess;

namespace aria2_common
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Aria2_remote_service()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}

using System.ServiceProcess;

namespace aria2c_service
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
                new aria2c_remote()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}

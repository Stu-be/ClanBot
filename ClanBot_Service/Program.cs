using System.ServiceProcess;

namespace ClanBot_Service
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
                new ClanBotService() 
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}

using ClanBot;
using System;
using System.ServiceProcess;
using System.Threading;

namespace ClanBot_Service
{
    public partial class ClanBotService : ServiceBase
    {
        Thread th;
        bool isRunning = false;
        private ClasherDynBot bot { get; set; }
        private LogWriter logger { get; set; }

        public ClanBotService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            logger = new LogWriter("SERVER STARTED - " + DateTime.Now);
            try
            {
                //System.Diagnostics.Debugger.Launch();
                th = new Thread(DoThis);
                th.Start();
                isRunning = true;
            }
            catch (Exception ex) { logger.LogWrite(DateTime.Now + " - " + ex.Message + " --- " + ex); }
        }

        public void DoThis()
        {
            while (isRunning)
            {
                try
                {
                    if (bot == null)
                    {
                        bot = new ClasherDynBot();
                    }
                    Thread.Sleep(5);
                }
                catch (Exception ex)
                {
                    logger.LogWrite(DateTime.Now + " - " + ex.Message + " --- " + ex);
                }

            }
        }

        protected override void OnStop()
        {
            isRunning = false;
        }
    }
}

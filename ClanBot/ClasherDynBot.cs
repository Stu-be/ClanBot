using ClanBot.Commands;
using Discord;
using Discord.Commands;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace ClanBot
{
    public class ClasherDynBot
    {
        DiscordClient discord;
        CommandService commands;

        Random rand;

        public ClasherDynBot()
        {
            rand = new Random();
            discord = new DiscordClient(x =>
            {
                x.LogLevel = LogSeverity.Info;
                x.LogHandler = Log;
            });

            discord.UsingCommands(x =>
            {
                x.PrefixChar = '!';
                x.AllowMentionPrefix = true;
            });

            commands = discord.GetService<CommandService>();

            #region register command groups
            HelpCommands.RegisterHelpCommands(commands, discord);
            FunCommands.RegisterFunCommands(commands, discord);
            SettingCommands.RegisterWebsiteCommands(commands, discord);          
            SearchCommands.RegisterSearchCommands(commands, discord);
            AttendanceCommands.RegisterAttendanceCommands(commands, discord);
            WarningCommands.RegisterWarningCommands(commands, discord);
            EnemyNoteCommands.RegisterEnemyNoteCommands(commands, discord);
            WarCommands.RegisterWarCommands(commands, discord);
            AdminCommands.RegisterAdminCommands(commands, discord);
            #endregion

            discord.ExecuteAndWait(async () =>
            {
                string key = ConfigurationManager.AppSettings["Key"];
                await discord.Connect(key, Discord.TokenType.Bot);
                //auto start notifications
                await StartNotifications();                 
            });         
        }

        private async Task StartNotifications()
        {
            //wait 5 seconds to ensure the bot is connected and setup
            await Task.Delay(5000);
            await NotificationManager.StartRegisterNotification(discord, 15);
        }

        private void Log(object sender, LogMessageEventArgs e)
        {
            //Console.WriteLine(DateTime.Now + " - " + e.Message + " - " + e.Exception);
            new LogWriter(DateTime.Now + " - " + e.Message + " --- " + e.Exception);
        }
    }
}

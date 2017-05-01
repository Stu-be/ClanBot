using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

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

            Help.RegisterHelpCommand(commands, discord);
            Help.RegisterShowMeTheCodeCommand(commands, discord);
            Help.RegisterShowRecruitMessageCommand(commands, discord);

            Fun.RegisterHelloCommand(commands, discord);
            Fun.RegisterSmellsCommand(commands, discord, rand);
            Fun.RegisterClanNoobsCommand(commands, discord);

            Admin.RegisterPurgeCommand(commands, discord);

            Search.RegisterSearchMembersCommand(commands, discord);

            Attendance.RegisterGetPlayerAttendanceCommand(commands, discord);
            Attendance.RegisterGetPlayerCurSeasonAttendanceCommand(commands, discord);

            WarCommands.RegisterDisplayWarInfoCommand(commands, discord);
            WarCommands.RegisterGetClaimedBasesCommand(commands, discord);
            WarCommands.RegisterBaseCheckCommand(commands, discord);

            EnemyBaseNotes.RegisterDisplayEnemyBaseNotes(commands, discord);
            EnemyBaseNotes.RegisterAddBaseNoteCommand(commands, discord);
            EnemyBaseNotes.RegisterRemoveBaseNoteCommand(commands, discord);

            Warnings.RegisterListWarningsCommand(commands, discord);
            Warnings.RegisterPlayerWarningCommand(commands, discord);
            Warnings.RegisterRemovePlayerWarningCommand(commands, discord);

            discord.ExecuteAndWait(async () =>
            {
                string key = ConfigurationManager.AppSettings["Key"];
                await discord.Connect(key, Discord.TokenType.Bot);
            });
        }

        private void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine(DateTime.Now + " - " + e.Message + " - " + e.Exception);
            new LogWriter(DateTime.Now + " - " + e.Message + " --- " + e.Exception);
        }
    }
}

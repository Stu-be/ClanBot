using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClanBot
{
    static class Help
    {
        public static void RegisterHelpCommand(CommandService commands, DiscordClient discord)
        {
            commands.CreateCommand("Help")
                .Description("!help - get a list of all clanbot commands.")
                .AddCheck((command, user, channel) => !user.IsBot)
                .Do(async (e) =>
                {
                    string text = "**CLANBOT COMMANDS**\n```";

                    foreach (Discord.Commands.Command c in commands.AllCommands)
                    {
                        if (c.IsHidden)
                            text += "" + c.Description.ToLower() + " **admin only**\n";
                        else
                            text += "" + c.Description.ToLower() + "\n";
                    }
                    text += "```";
                    await e.Channel.SendMessage(text);

                });
        }

        public static void RegisterShowMeTheCodeCommand(CommandService commands, DiscordClient discord)
        {
            commands.CreateCommand("Show me the code")
                .Description("!show me the code - to view clanbot code.")
                .AddCheck((command, user, channel) => !user.IsBot)
                .Do(async (e) =>
                {
                    string text = "You can find me here https://github.com/Stu-be/ClanBot_Service/";

                    await e.Channel.SendMessage(text);

                });
        }

        public static void RegisterShowRecruitMessageCommand(CommandService commands, DiscordClient discord)
        {
            commands.CreateCommand("Recruit message")
                .Description("!recruit message - to view a recruit message.")
                .AddCheck((command, user, channel) => !user.IsBot)
                .Do(async (e) =>
                {
                    string text = "** RECRUIT MESSAGE ** \n ```** RECRUITING: non-rushed TH9+ * Adults only * 3 wars p/w * Website used for war & stat tracking **```"; //99 characters

                    await e.Channel.SendMessage(text);

                });
        }
    }
}

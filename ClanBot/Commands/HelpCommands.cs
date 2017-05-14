using Discord;
using Discord.Commands;
using System;

namespace ClanBot.Commands
{
    static class HelpCommands
    {
        public static void RegisterHelpCommands(CommandService commands, DiscordClient discord)
        {
            commands.CreateGroup("help", cgb =>
            {
                cgb.Category("=== HELP COMMANDS ===");
                cgb.CreateCommand("")
                    .Description("!help - get a list of all clanbot commands.")
                    .AddCheck((command, user, channel) => !user.IsBot)
                    .Do(async (e) =>
                    {
                        #region help
                        try
                        {
                            string lastCmd = null;
                            string text = "**CLANBOT COMMANDS**\n```";
                            foreach (Command c in commands.AllCommands)
                            {
                                if (c.Category != null)
                                {
                                    if (c.Category != lastCmd)
                                    {
                                        lastCmd = c.Category;
                                        text += "\n"+c.Category.ToUpper() + "\n";
                                    }
                                }
                                else
                                {
                                    if (lastCmd != "=== NOT GROUPED ===")
                                    {
                                        lastCmd = "=== NOT GROUPED ===";
                                        text += "\n" + lastCmd + "\n";
                                    }
                                }
                                if (!c.IsHidden)
                                    text += "" + c.Description.ToLower() + "\n";
                            }
                            text += "```";
                            await e.User.PrivateChannel.SendMessage(text);
                            await e.Channel.SendMessage("**HELP**```THATS WHAT IM HERE FOR!!! A list of commands is being sent via private message.```");
                        }
                        catch (Exception ex)
                        {
                            discord.Log.Error("Help.RegisterHelpCommands,help", ex.Message);
                        }
                        #endregion
                    });

                cgb.CreateCommand("code")
                    .Description("!help code - to view clanbot code.")
                    .Alias(new string[] { "-c" })
                    .AddCheck((command, user, channel) => !user.IsBot)
                    .Do(async (e) =>
                    {
                        #region help code
                        try
                        {
                            string text = "You can find me here https://github.com/Stu-be/ClanBot/";

                            await e.Channel.SendMessage(text);
                        }
                        catch (Exception ex)
                        {
                            discord.Log.Error("Help.RegisterHelpCommands.code", ex.Message);
                        }
                        #endregion
                    });

                cgb.CreateCommand("Recruit")
                    .Description("!help recruit - to view a recruit message you can paste in game.")
                    .Alias(new string[] { "-r" })
                    .AddCheck((command, user, channel) => !user.IsBot)
                    .Do(async (e) =>
                    {
                        #region help recruit
                        try
                        {
                            string text = "** RECRUIT MESSAGE ** \n ```** RECRUITING: non-rushed TH9+ * Adults only * 3 wars p/w * Website used for war & stat tracking **```"; //99 characters

                            await e.Channel.SendMessage(text);
                        }
                        catch (Exception ex)
                        {
                            discord.Log.Error("Help.RegisterHelpCommands.recruit", ex.Message);
                        }
                        #endregion
                    });
            });
        }
    }
}

using Discord;
using Discord.Commands;
using System;

namespace ClanBot.Commands
{
    static class FunCommands
    {
        public static void RegisterFunCommands(CommandService commands, DiscordClient discord)
        {
            commands.CreateGroup("", cgb =>
            {
                cgb.Category("=== FUN COMMANDS ===");
                cgb.CreateCommand("hello")
                    .Description("!hello - say hello to clanbot.")
                    .Alias(new string[] { "hi", "hey", "lo" })
                    .AddCheck((command, user, channel) => !user.IsBot)
                    .Do(async (e) =>
                    {
                        #region hello
                        try
                        {
                            await e.Channel.SendMessage("Hello " + e.User.Name + " :)");
                        }
                        catch (Exception ex)
                        {
                            discord.Log.Error("Help.RegisterHelpCommands", ex.Message);
                        }
                        #endregion
                    });

                cgb.CreateCommand("smells")
                    .Description("!smells - tell clanbot how much it smells!")
                    .Alias(new string[] { "stinks", "smelly", "you smell" })
                    .AddCheck((command, user, channel) => !user.IsBot)
                    .Do(async (e) =>
                    {
                        #region smells
                        Random rand = new Random();
                        if (!e.User.IsBot)
                        {
                            try
                            {
                                int answer = rand.Next(4);
                                switch (answer)
                                {
                                    case 0:
                                        await e.Channel.SendMessage("Damn really? i'll go take a shower! :dash:");
                                        break;
                                    case 1:
                                        await e.Channel.SendMessage("Sorry, i melt like the witch from Wizard of Oz when i touch water!");
                                        break;
                                    case 2:
                                        await e.Channel.SendMessage("Be nice :rage:");
                                        break;
                                    case 3:
                                        await e.Channel.SendMessage("But im not due a shower till my birthday! :birthday:");
                                        break;
                                }
                            }
                            catch (Exception ex) { discord.Log.Error("RegisterGetClaimedBasesCommand", ex.Message); }
                        }
                        #endregion
                    });

                cgb.CreateCommand("Noob")
                    .Description("!noob [name] <name> <name> - list one, two or three clan noobs! ")
                    .Alias(new string[] { "n00b", "newbie", "newb" })
                    .AddCheck((command, user, channel) => !user.IsBot)
                    .Parameter("noob1", ParameterType.Required)
                    .Parameter("noob2", ParameterType.Optional)
                    .Parameter("noob3", ParameterType.Optional)
                    .Do(async (e) =>
                    {
                        #region noob
                        string msg = "**CLAN NEWBIES aka.. n00bs, newb** \n ";
                        msg += "```nOOb >> " + e.GetArg(0) + " << NOOB```\n";
                        if(e.GetArg(1) != "")
                            msg += "```nOOb >> " + e.GetArg(1) + " << NOOB```\n";
                        if (e.GetArg(2) != "")
                            msg += "```nOOb >> " + e.GetArg(2) + " << NOOB```\n";
                        await e.Channel.SendMessage(msg);
                        #endregion
                    });
            });
        }
    }
}

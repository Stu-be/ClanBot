using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClanBot
{
    static class Fun
    {
        public static void RegisterHelloCommand(CommandService commands, DiscordClient discord)
        {
            commands.CreateCommand("Hello")
                .Description("!hello - say hello to clanbot.")
                .AddCheck((command, user, channel) => !user.IsBot)
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("Hello " + e.User.Name + " :)");
                });
        }

        public static void RegisterSmellsCommand(CommandService commands, DiscordClient discord, Random rand)
        {
            commands.CreateCommand("Smells")
                .Description("!smells - tell clanbot how much he smells!")
                .AddCheck((command, user, channel) => !user.IsBot)
                .Do(async (e) =>
                {
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
                });
        }

        public static void RegisterClanNoobsCommand(CommandService commands, DiscordClient discord)
        {
            commands.CreateCommand("Clan Noobs")
                .Description("!clan noobs [name] [name] [name] - list 3 memebers who are the clan noobs!")
                .AddCheck((command, user, channel) => !user.IsBot)
                .Parameter("noob1", ParameterType.Required)
                .Parameter("noob2", ParameterType.Required)
                .Parameter("noob3", ParameterType.Required)
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("***CLAN NOOBS*** \n " + "```NOOB >> " + e.GetArg(0) + " << NOOB``` \n ```NOOB >> " + e.GetArg(1) + " << NOOB``` \n ```NOOB >> " + e.GetArg(2) + " << NOOB```");
                });
        }
    }
}

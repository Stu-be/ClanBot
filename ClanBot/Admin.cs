using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClanBot
{
    static class Admin
    {
        public static void RegisterPurgeCommand(CommandService commands, DiscordClient discord)
        {
            commands.CreateCommand("Purge")
                .Description("!purge - clear all excluding the newest 50 messages.")
                .AddCheck((command, user, channel) => !user.IsBot)
                .AddCheck((cm, u, ch) => u.ServerPermissions.Administrator)
                .Hide()
                .Do(async (e) =>
                {
                    try
                    {
                        if (e.Message.User.Roles.First().Name == "@Co-Leader")
                        {
                            await e.Channel.SendMessage("CLEARING OLD MESSAGES. PLEASE BE PATIENT! ");
                            Message[] messages;
                            int deleted = 0;
                            messages = await e.Channel.DownloadMessages(100);

                            List<Message> messagesToDelete = new List<Message>();
                            while (messages.Length > 50)
                            {
                                for (int i = messages.Length; i > 50; i--)
                                {
                                    messagesToDelete.Add(messages[i - 1]);
                                    deleted++;
                                }
                                await e.Channel.DeleteMessages(messagesToDelete.ToArray());
                                messagesToDelete = new List<Message>();
                                messages = await e.Channel.DownloadMessages(100);
                            }

                            await e.Channel.SendMessage(deleted + " MESSAGES CLEARED");
                        }
                        else
                        {
                            await e.Channel.SendMessage("YOU DO NOT HAVE PERMISSION! " + e.Message.User.Roles.First());
                        }
                    }
                    catch (Exception ex) { Console.WriteLine("ERROR! - " + ex.Message); }
                });
        }
    }
}

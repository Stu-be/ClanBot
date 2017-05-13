using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClanBot.Commands
{
    static class AdminCommands
    {
        
        public static void RegisterAdminCommands(CommandService commands, DiscordClient discord)
        {
            commands.CreateGroup("admin", cgb =>
            {
                cgb.Category("=== ADMIN ONLY COMMANDS ===");
                cgb.CreateCommand("purge")
                        .Description("!admin purge - clear all messages excluding the newest 50.")
                        .Alias(new string[] { "-p" })
                        .AddCheck((command, user, channel) => !user.IsBot)
                        .AddCheck((cm, u, ch) => u.ServerPermissions.Administrator)
                        .Do(async (e) =>
                        {
                            #region admin purge
                            try
                            {
                                if (e.Message.User.Roles.First().Name == "@Co-Leader")
                                {
                                    await e.Channel.SendMessage("CLEARING OLD MESSAGES. PLEASE BE PATIENT!");
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
                                        await Task.Delay(2000);
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
                            #endregion
                        });

                cgb.CreateCommand("register notification")
                    .Description("!admin register notification [on/off] <interval> - toggle register notifications on/off.")
                    .Alias(new string[] { "-rn" })
                    .AddCheck((command, user, channel) => !user.IsBot)
                    .AddCheck((cm, u, ch) => u.HasRole(discord.Servers.FirstOrDefault().FindRoles("@Co-Leader").FirstOrDefault()))
                    .Parameter("toggle", ParameterType.Required)
                    .Parameter("interval", ParameterType.Optional)
                    .Do(async (e) =>
                    {
                        #region admin register notification
                        try
                        {
                            if (e.Message.User.Roles.First().Name == "@Co-Leader")
                            {
                                int interval = 15;
                                if (e.GetArg("interval") != null || e.GetArg("interval") != string.Empty)
                                {
                                    int.TryParse(e.GetArg("interval"), out interval);
                                    if (interval < 5 || interval > 300)
                                    {
                                        interval = 15;
                                    }
                                }
                                Channel channel = discord.Servers.FirstOrDefault().FindChannels("leaders_chat", ChannelType.Text, false).FirstOrDefault();

                                if (e.GetArg("toggle").ToString().ToLower() == "on")
                                {
                                    await NotificationManager.StartRegisterNotification(discord, interval, e);
                                }
                                else if (e.GetArg("toggle").ToString().ToLower() == "off")
                                {
                                    await NotificationManager.StopRegisterNotification(discord, e);
                                }
                            }
                            else
                            {
                                await e.Channel.SendMessage("YOU DO NOT HAVE PERMISSION! " + e.Message.User.Roles.First());
                            }
                        }
                        catch (Exception ex)
                        {
                            discord.Log.Error("RegisterNewMemberNotification.StartRegisterNotification", ex.Message);
                        }
                        #endregion
                    });
               
            });
        }
    }
}

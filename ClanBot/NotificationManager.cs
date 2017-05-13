using ClanBot.ResultObjects;
using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClanBot
{

    public static class NotificationManager
    {
        public enum NotificationType
        {
            NEW_MEMBER, CLAIMED_BASE, RESULT_ENTERED
        }

        static List<Task> notifications { get; set; }
        static Task registerCheckTask { get; set; }
        static CancellationTokenSource registerCheckTaskToken { get; set; }
        public static List<RegisterNotification> registerNotificationsSent = new List<RegisterNotification>();
      
        public static async Task StartRegisterNotification(DiscordClient discord, int interval, CommandEventArgs e = null)
        {
            try
            {
                Channel channel = discord.Servers.FirstOrDefault().FindChannels("leaders_chat", ChannelType.Text, false).FirstOrDefault();

                if (registerCheckTask == null)
                {
                    registerCheckTaskToken = new CancellationTokenSource();
                    registerCheckTask = Repeat.Interval(
                    TimeSpan.FromSeconds(interval),
                    async
                    () =>
                    {
                        ClasherDynastyDataContext dc = new ClasherDynastyDataContext();
                        List<string> newMembers = (from u in dc.Users
                                                    from mem in dc.Memberships
                                                        .Where(me => u.UserId == me.UserId).DefaultIfEmpty()
                                                    from uir in dc.UsersInRoles
                                                        .Where(ui => u.UserId == ui.UserId).DefaultIfEmpty()
                                                    where uir.Role == null
                                                    select u.UserName).ToList();

                        if (newMembers.Count > 0)
                        {
                            bool postNotification = false;
                            string msg = "**NEW MEMBER REGISTRATION**\n```";
                            foreach (string n in newMembers)
                            {
                                bool onCooldown = false;
                                foreach (RegisterNotification rn in registerNotificationsSent)
                                {
                                    if (rn.user == n)
                                    {
                                        if (DateTime.Now > rn.lastNotificationTime.AddMinutes(rn.intervalMin))
                                        {
                                            registerNotificationsSent.Remove(rn);
                                            break;
                                        }
                                        else
                                        {
                                            onCooldown = true;
                                            break;
                                        }
                                    }

                                }
                                if (!onCooldown)
                                {
                                    msg += n + " registered and is awaiting approval... \n";
                                    postNotification = true;
                                    registerNotificationsSent.Add(new RegisterNotification { user = n, lastNotificationTime = DateTime.Now, intervalMin = 15 });
                                }

                            }
                            msg += "```";
                            if (postNotification)
                                await channel.SendMessage(msg);
                        }

                    }, registerCheckTaskToken.Token);

                    if(e != null)
                        await e.Channel.SendMessage("NOTIFICATION FOR REGISTERED MEMBERS IS NOW ON. CHECK INTERVAL SET TO " + interval + " SEC");
                }
                else
                {
                    if (e != null)
                        await e.Channel.SendMessage("NOTIFICATION FOR REGISTERED MEMBERS IS ALREADY ON!");
                }
        
            }
            catch (Exception ex)
            {
                discord.Log.Error("Notification.StartRegisterNotification", ex.Message);
            }
        }

        public static async Task StopRegisterNotification(DiscordClient discord, CommandEventArgs e = null)
        {
            try
            {
                if (registerCheckTask != null)
                {
                    if (registerCheckTask.Status == TaskStatus.Running)
                    {
                        registerCheckTaskToken.Cancel();
                        registerCheckTask = null;
                        registerNotificationsSent.Clear();
                        if (e != null)
                            await e.Channel.SendMessage("NOTIFICATION FOR REGISTERED MEMBERS IS NOW OFF.");
                    }
                    else
                    {
                        if (e != null)
                            await e.Channel.SendMessage("NOTIFICATION FOR REGISTERED MEMBERS IS ALREADY OFF!");
                    }
                }
                else
                {
                    if (e != null)
                        await e.Channel.SendMessage("NOTIFICATION FOR REGISTERED MEMBERS IS ALREADY OFF!");
                }
            }
            catch (Exception ex)
            {
                discord.Log.Error("Notification.StartRegisterNotification", ex.Message);
            }
        }
    }

    internal static class Repeat
    {
        public static Task Interval(
            TimeSpan pollInterval,
            Action action,
            CancellationToken token)
        {
            // We don't use Observable.Interval:
            // If we block, the values start bunching up behind each other.
            return Task.Factory.StartNew(
                () =>
                {
                    for (; ; )
                    {
                        if (token.WaitCancellationRequested(pollInterval))
                            break;

                        action();
                    }
                }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }
    }

    static class CancellationTokenExtensions
    {
        public static bool WaitCancellationRequested(
            this CancellationToken token,
            TimeSpan timeout)
        {
            return token.WaitHandle.WaitOne(timeout);
        }
    }

}

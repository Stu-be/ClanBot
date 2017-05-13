using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;

namespace ClanBot.Commands
{
    class SettingCommands
    {
        /// <summary>
        /// List of current two part commands. to store reference to original message event
        /// </summary>
        private static List<ResultObjects.TwoPartCommand> currentCommands { get; set; }

        /// <summary>
        /// Handles the register of all setting commands with the discord server
        /// </summary>
        /// <param name="commands">CommandService pointer</param>
        /// <param name="discord">DiscordClient pointer</param>
        public static void RegisterWebsiteCommands(CommandService commands, DiscordClient discord)
        {
            //create group for settings commands
            commands.CreateGroup("set", cgb =>
            {
                cgb.Category("=== SETTING COMMANDS ===");
                //create link command to link discord account to website account
                cgb.CreateCommand("link")
                    .Description("!set link - link your discord to the website, allowing website commands.")
                    .Alias(new string[] { "-l" })
                    .AddCheck((command, user, channel) => !user.IsBot)
                    .Parameter("username", ParameterType.Optional)
                    .Parameter("password", ParameterType.Optional)
                    .Do(async (e) =>
                    {
                        #region set link
                        List<Message> myMessages = new List<Message>();
                        Message[] messages = null;
                        //if message came from public channel
                        if (!e.Channel.IsPrivate)
                        {
                            if (currentCommands == null)
                                currentCommands = new List<ResultObjects.TwoPartCommand>();
                            List<ResultObjects.TwoPartCommand> oldCommands = new List<ResultObjects.TwoPartCommand>();
                            //check is the user has any incomple two part commands
                            foreach (ResultObjects.TwoPartCommand tpc in currentCommands)
                            {
                                if (tpc.user.Id == e.User.Id)
                                {
                                    oldCommands.Add(tpc);
                                }
                            }
                            //remove the incomplete commands
                            currentCommands.RemoveAll(item => oldCommands.Contains(item));
                            //add this new command so we can get the origin message event 
                            currentCommands.Add(new ResultObjects.TwoPartCommand() { command = e, channel = e.Channel, user = e.User });

                            //if someone posts username and password into public channel, delete it ASAP
                            if (e.GetArg(0) != "" || e.GetArg(1) != "")
                            {
                                try
                                {
                                    messages = await e.Channel.DownloadMessages(25);
                                    messages = messages.OrderByDescending(x => x.Timestamp).ToArray();
                                    //currenly just gets last message.. this needs improving to check for user of last message
                                    myMessages.Add(messages[0]);
                                    await e.Channel.DeleteMessages(myMessages.ToArray());
                                    myMessages.Clear();

                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("ERROR! - " + ex.Message);
                                }
                                await e.Message.Channel.SendMessage("**WARNING** ```DO NOT POST YOU WEBSITE LOGIN IN A PUBLIC CHANNEL. IF THE DETAILS WERE CORRECT, YOU SHOULD CHANGE YOUR PASSWORD ASAP!...```");
                            }

                            await e.Message.Channel.SendMessage("**WEBSITE LINK STARTED** ```PLEASE COMPLETE VIA PRIVATE MESSAGE...```");
                            await e.User.PrivateChannel.SendMessage("```WEBSITE LINK WARNING: only ever attempt this via private message. linking from a chat channel will reveal your password! \n"
                                                                                    + " To link discord to the website repeat the previous command with the addition of your website username and password. \n"
                                                                                    + " e.g !set link <website_username> <website_password>```");
                        } 
                        //if message is private message
                        else
                        {
                            try
                            {
                                bool success = false;
                                //attempt to select user with entered username
                                ClasherDynastyDataContext dc = new ClasherDynastyDataContext();
                                User user = (from u in dc.Users
                                                where u.UserName == e.GetArg(0)
                                                select u).FirstOrDefault();
                                //if user exists
                                if (user != null)
                                {
                                    //hash, salt and check passwords is correct 
                                    if (Encode.EncodePassword(e.GetArg(1), MembershipPasswordFormat.Hashed, user.Membership.PasswordSalt) == user.Membership.Password)
                                    {
                                        //if user is not linked
                                        if (user.DiscordId == null)
                                        {
                                            //link discord id 
                                            user.DiscordId = e.User.Id.ToString();
                                            try
                                            {
                                                dc.SubmitChanges();
                                                success = true;
                                            }
                                            catch (Exception ex)
                                            {
                                                discord.Log.Error("Admin.RegisterLinkAccountCommand: error submitting changes.", ex.Message);
                                            }
                                            if (success)
                                            {
                                                await e.User.PrivateChannel.SendMessage("```WEBSITE LINK SUCCESSFUL: you can now claim bases on the website and much more...!```");
                                                if (currentCommands.Find(item => e.User.Id == item.user.Id).channel != null)
                                                    await currentCommands.Find(item => e.User.Id == item.user.Id).channel.SendMessage("**WEBSITE LINK COMPLETE** ```" + e.User.Name.ToUpper() + " IS POWERED UP AND READY!!```");
                                            }

                                        }
                                        //if user is already linked to that account
                                        else if (user.DiscordId == e.User.Id.ToString())
                                        {
                                            success = true;
                                            await e.User.PrivateChannel.SendMessage("```WEBSITE LINK SUCCESSFUL: that account is already linked!```");
                                            if (currentCommands.Find(item => e.User.Id == item.user.Id).channel != null)
                                                await currentCommands.Find(item => e.User.Id == item.user.Id).channel.SendMessage("**WEBSITE LINK COMPLETE** ```" + e.User.Name.ToUpper() + " IS POWERED UP AND READY!!```");

                                        }
                                        //if user is linked but to a differant discord account
                                        else
                                        {
                                            await e.User.PrivateChannel.SendMessage("```WEBSITE LINK FAILED: that account is already linked!```");
                                        }

                                    }
                                    //if incorrect password
                                    else
                                    {
                                        await e.User.PrivateChannel.SendMessage("```WEBSITE LINK FAILED: please check your username and password are correct!```");
                                    }
                                }
                                //if incorrect username 
                                else
                                {
                                    await e.User.PrivateChannel.SendMessage("```WEBSITE LINK FAILED: user not found!```");
                                }

                                //if account link failed for any reason
                                if (!success)
                                {
                                    if (currentCommands != null)
                                        if (currentCommands.Find(item => e.User.Id == item.user.Id).channel != null)
                                            await currentCommands.Find(item => e.User.Id == item.user.Id).channel.SendMessage("**WEBSITE LINK CANCELED** ```PLEASE CHECK PRIVATE MESSAGE FOR DETAILS.```");
                                }

                            }
                            catch (Exception ex)
                            {
                                discord.Log.Error("Admin.RegisterLinkAccountCommand", ex.Message);
                            }
                        }
                        #endregion
                    });
            });
        }
    }
}

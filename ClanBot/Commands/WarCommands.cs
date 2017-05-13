using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClanBot.Commands
{
    static class WarCommands
    {
        public static void RegisterWarCommands(CommandService commands, DiscordClient discord)
        {
            commands.CreateGroup("war", cgb =>
            {
                cgb.Category("=== WAR COMMANDS ===");
                cgb.CreateCommand("info")
                    .Description("!war info - list all war bases.")
                    .Alias(new string[] { "-i", "" })
                    .AddCheck((command, user, channel) => !user.IsBot)
                    .Do(async (e) =>
                    {
                        #region war info
                        try
                        {
                            ClasherDynastyDataContext dc = new ClasherDynastyDataContext();
                            List<EnemyBasesView> res = new List<EnemyBasesView>();
                            res = (from ebv in dc.EnemyBasesViews select ebv).ToList();

                            string message = "WAR INFO: \n";
                            foreach (EnemyBasesView eb in res)
                            {
                                if (eb.STARS == 3)
                                {
                                    message += "`#" + eb.BASE__
                                                    + ((eb.CLAIMED_BY == null) ? " | FLAT" : " | CLAIMED BY: " + eb.CLAIMED_BY)
                                                    + ((eb.ATTACKS == 0) ? "" : (" | ATTACKS: " + eb.ATTACKS
                                                    + " | BEST ATTACK: " + eb.BEST
                                                    + " | STARS: " + eb.STARS
                                                    + " | DAMAGE: " + eb.DAMAGE + "%")) + "`\n";
                                }
                                else
                                {
                                    message += "`#" + eb.BASE__
                                                        + ((eb.CLAIMED_BY == null) ? " | FREE" : " | CLAIMED BY: " + eb.CLAIMED_BY)
                                                        + ((eb.ATTACKS == 0) ? "" : (" | ATTACKS: " + eb.ATTACKS
                                                        + " | BEST ATTACK: " + eb.BEST
                                                        + " | STARS: " + eb.STARS
                                                        + " | DAMAGE: " + eb.DAMAGE + "%")) + "`\n";
                                }

                            }

                            await e.Channel.SendMessage(message);
                        }
                        catch (Exception ex) { discord.Log.Error("RegisterDisplayWarInfoCommand", ex.Message); }
                        #endregion
                    });

                cgb.CreateCommand("claimed")
                    .Description("!war claimed - get a list of claimed war bases.")
                    .Alias(new string[] { "-c" })
                    .AddCheck((command, user, channel) => !user.IsBot)
                    .Do(async (e) =>
                    {
                        #region war claimed bases
                        try
                        {
                            ClasherDynastyDataContext dc = new ClasherDynastyDataContext();
                            List<EnemyBasesView> res = new List<EnemyBasesView>();
                            res = (from ebv in dc.EnemyBasesViews select ebv).ToList();

                            string message = "**CLAIMED WAR BASES** \n";
                            foreach (EnemyBasesView eb in res)
                            {
                                if (eb.CLAIMED_BY != null && eb.CLAIMED_BY != "")
                                {
                                    TimeSpan t = TimeSpan.FromSeconds((int)eb.DATETIME);
                                    string timepassed = string.Format("{0:D2}h:{1:D2}m:{2:D2}s", t.Hours, t.Minutes, t.Seconds);
                                    message += "```#" + eb.BASE__ + (((int)eb.BASE__ < 10) ? "  | CLAIMED BY: " : " | CLAIMED BY: ")
                                                        + eb.CLAIMED_BY + " | CLAIMED FOR: " + timepassed + "\n"
                                                        + ((eb.ATTACKS == 0) ? "" : ("    | ATTACKS: " + eb.ATTACKS
                                                        + " | BEST ATTACK: " + eb.BEST
                                                        + " | STARS: " + eb.STARS
                                                        + " | DAMAGE: " + eb.DAMAGE + "%")) + "```";
                                }
                            }
                            message += "";
                            await e.Channel.SendMessage(message);
                        }
                        catch (Exception ex) { discord.Log.Error("RegisterGetClaimedBasesCommand", ex.Message); }
                        #endregion
                    });

                cgb.CreateCommand("enemy")
                    .Description("!war enemy [base number] - show details of the requested war base.")
                    .Alias(new string[] { "-e" })
                    .AddCheck((command, user, channel) => !user.IsBot)
                    .Parameter("Base", ParameterType.Required)
                    .Do(async (e) =>
                    {
                        #region war enemy info
                        bool sucess = false;
                        try
                        {
                            ClasherDynastyDataContext dc = new ClasherDynastyDataContext();
                            EnemyBasesView ebv = new EnemyBasesView();
                            ebv = (from eb in dc.EnemyBasesViews
                                   where eb.BASE__.Equals(e.GetArg("Base"))
                                   select eb).FirstOrDefault();
                            if (ebv == null)
                            {
                                await e.Channel.SendMessage("INVALID BASE NUMBER OR NO WAR IN PROGRESS");
                            }
                            else
                            {
                                await e.Channel.SendMessage("BASE: #" + ebv.BASE__
                                                    + ((ebv.CLAIMED_BY == null) ? " | NOT CLAIMED" : " | CLAIMED BY: " + ebv.CLAIMED_BY)
                                                    + ((ebv.ATTACKS == 0) ? "" : (" | ATTACKS: " + ebv.ATTACKS
                                                    + " | BEST ATTACK: " + ebv.BEST
                                                    + " | STARS: " + ebv.STARS
                                                    + " | DAMAGE: " + ebv.DAMAGE + "%"))
                                                    + ((ebv.NOTES == null) ? "" : (" | NOTES: " + ebv.NOTES)));
                            }
                            sucess = true;
                        }
                        catch (Exception ex) { sucess = false; throw ex; }
                        if (!sucess)
                        {
                            await e.Channel.SendMessage("THAT COMMAND IS NOT AVAILABLE RIGHT NOW!");

                        }
                        #endregion
                    });

                cgb.CreateCommand("damage")
                    .Description("!war damage - calculate and list available damage from remaining bases.")
                    .Alias(new string[] { "-d", "dmg" })
                    .AddCheck((command, user, channel) => !user.IsBot)
                    .Do(async (e) =>
                    {
                        #region war damage
                        try
                        {
                            ClasherDynastyDataContext dc = new ClasherDynastyDataContext();
                            List<EnemyBasesView> ebv = new List<EnemyBasesView>();
                            ebv = (from eb in dc.EnemyBasesViews
                                   select eb).ToList();
                            if (ebv != null)
                            {
                                double totaldamage = 0;
                                int warSize = ebv.Count();

                                string result = "**AVAILABLE DAMAGE**\n";
                                foreach (EnemyBasesView eb in ebv)
                                {
                                    double baseDmg = (100 / warSize) * (eb.DAMAGE ?? 0);
                                    totaldamage += baseDmg;
                                    if(eb.DAMAGE != 100)
                                    {
                                        if (eb.BASE__ < 10)
                                            result += "`#" + eb.BASE__ + " ";
                                        else
                                            result += "`#" + eb.BASE__;
                                        result +=  " | STARS: " + (eb.STARS ?? 0) + " | DAMAGE: " + (eb.DAMAGE ?? 0) + "% | AVAILABLE OVERALL DAMAGE: " + Math.Round(((100.00 / warSize) / 100) * (100.00 - (double)(eb.DAMAGE ?? 0.00)),2) + "%`\n";
                                    }
                                }
                                await e.Channel.SendMessage(result);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        #endregion
                    });

                cgb.CreateCommand("claim target")
                    .Description("!war claim target [base number] - claim a war base. **note: must have acount linked**")
                    .Alias(new string[] { "-ct", "claim" })
                    .AddCheck((command, user, channel) => !user.IsBot)
                    .Parameter("baseNumber", ParameterType.Required)
                    .Do(async (e) =>
                    {
                        #region war claim a target
                        try
                        {
                            ClasherDynastyDataContext dc = new ClasherDynastyDataContext();
                            User user = (from u in dc.Users
                                         where u.DiscordId == e.User.Id.ToString()
                                         select u).FirstOrDefault();

                            if (user != null)
                            {
                                await e.Channel.SendMessage("**CLAIM BASE** ```TARGET CLAIMED: #" + e.GetArg(0) + " watch out! " + user.UserName + " is coming for you.```");
                                await e.Channel.SendMessage("```NOT YET IMPLEMENTED, BUT IF YOU SEE THIS MESSAGE YOU ARE SETUP AND READY :)```");
                            }
                            else
                            {
                                await e.Channel.SendMessage("**CLAIM BASE** ```FAILED TO CLAIM: account is not linked!```");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("ERROR! - " + ex.Message);
                        }
                        #endregion
                    });
            });
        }
    }
}

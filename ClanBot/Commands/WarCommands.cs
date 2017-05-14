using ClanBot.ResultObjects;
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
                                int? returnValue = dc.ClaimBase(user.UserId.ToString(), e.GetArg(0));

                                switch (returnValue)
                                {
                                    case 1:         // Claim successful.
                                        String msg = "TARGET CLAIMED: #" + e.GetArg(0) + " watch out! " + user.UserName + " is coming for you.";
                                        LogWarNews(msg, DateTime.Now, user.UserName + "via ClanBot");
                                        await e.Channel.SendMessage("**CLAIM BASE** ```"+ msg +"```");
                                        break;
                                    case 51000:     // Both attacks already used.
                                        await e.Channel.SendMessage("**CLAIM BASE** ```CLAIM FAILED: sorry " + user.UserName + " you have already used both of your attacks this war.```");
                                        break;
                                    case 51001:     // Base is already claimed.
                                        await e.Channel.SendMessage("**CLAIM BASE** ```CLAIM FAILED: sorry " + user.UserName + " that base is already claimed.```");
                                        break;
                                    case 51002:     // Cannot hit the same base twice.
                                        await e.Channel.SendMessage("**CLAIM BASE** ```CLAIM FAILED: sorry " + user.UserName + " you cannot attack the same base twice.```");
                                        break;
                                    case 0:         // error claiming
                                        await e.Channel.SendMessage("**CLAIM BASE** ```CLAIM FAILED: sorry " + user.UserName + " unable to claim base at the moment... please use the website.```");
                                        break;
                                }
                            }
                            else
                            {
                                await e.Channel.SendMessage("**CLAIM BASE** ```CLAIM FAILED: account is not linked!```");
                            }
                        }
                        catch (Exception ex)
                        {
                            discord.Log.Error("RegisterDisplayWarInfoCommand", ex.Message);
                        }
                        #endregion
                    });

                cgb.CreateCommand("unclaim target")
                    .Description("!war unclaim target [base number] - unclaim a war base. **note: must have acount linked**")
                    .Alias(new string[] { "-ut", "unclaim" })
                    .AddCheck((command, user, channel) => !user.IsBot)
                    .Parameter("baseNumber", ParameterType.Required)
                    .Do(async (e) =>
                    {
                        #region war unclaim target
                        try
                        {
                            ClasherDynastyDataContext dc = new ClasherDynastyDataContext();
                            User user = (from u in dc.Users
                                         where u.DiscordId == e.User.Id.ToString()
                                         select u).FirstOrDefault();

                            if (user != null)
                            {
                                CurWarAttack attack = (from a in dc.CurWarAttacks
                                              where a.UserID == user.UserId
                                                && a.EnemyBase.EnemyBaseNo.ToString() == e.GetArg(0)
                                              select a).FirstOrDefault();

                                if(attack != null)
                                {
                                    dc.CurWarAttacks.DeleteOnSubmit(attack);
                                    dc.SubmitChanges();
                                    String msg = "TARGET UNCLAIMED: #" + e.GetArg(0) + " you're lucky! " + user.UserName + " has decided not to smash your base.";
                                    LogWarNews(msg, DateTime.Now, user.UserName + "via ClanBot");
                                    await e.Channel.SendMessage("**UNCLAIM BASE** ```" + msg + "```");
                                }
                                else
                                {
                                    await e.Channel.SendMessage("**UNCLAIM BASE** ```UNCLAIM FAILED: sorry " + user.UserName + " you do not seem to have claimed or attacked that base.```");
                                }
                                
                            }
                            else
                            {
                                await e.Channel.SendMessage("**UNCLAIM BASE** ```UNCLAIM FAILED: account is not linked!```");
                            }
                        }
                        catch (Exception ex)
                        {
                            discord.Log.Error("RegisterDisplayWarInfoCommand", ex.Message);
                        }
                        #endregion
                    });

                cgb.CreateCommand("enter result")
                    .Description("!war enter result [base number] [stars] [attack_type] [damage] - enter attack results. **note: must have acount linked**")
                    .Alias(new string[] { "-er", "result" })
                    .AddCheck((command, user, channel) => !user.IsBot)
                    .Parameter("baseNumber", ParameterType.Required)
                    .Parameter("numOfStars", ParameterType.Required)
                    .Parameter("attackType", ParameterType.Required)
                    .Parameter("damagePerc", ParameterType.Required)
                    .Do(async (e) =>
                    {
                        #region war enter result
                        try
                        {
                            ClasherDynastyDataContext dc = new ClasherDynastyDataContext();
                            User user = (from u in dc.Users
                                         where u.DiscordId == e.User.Id.ToString()
                                         select u).FirstOrDefault();

                            // if user is linked
                            if (user != null)
                            {
                                CurWarAttack attack = (from a in dc.CurWarAttacks
                                                       where a.UserID == user.UserId
                                                         && a.EnemyBase.EnemyBaseNo.ToString() == e.GetArg(0)
                                                       select a).FirstOrDefault();

                                // if claim or attack found for specified base
                                if (attack != null)
                                {
                                    // if no resul entered
                                    if (attack.Damage == null)
                                    {
                                        // validate stars attack type and damage
                                        AttackValidationResult res = ValidateResults(e.GetArg(1), e.GetArg(2), e.GetArg(3));
                                        if(res.success)
                                        {
                                            attack.Stars = res.stars;
                                            attack.AttackType = res.attackType;
                                            attack.Damage = res.damage;
                                            dc.SubmitChanges();

                                            String msg = " ATTACK: " + user.UserName + " just smashed #" + e.GetArg(0) + " for " + e.GetArg(1) + " stars and " + e.GetArg(3) + "% damage with " + e.GetArg(2) + ".";
                                            LogWarNews(msg, DateTime.Now, user.UserName + "via ClanBot");
                                            await e.Channel.SendMessage("**ENTER RESULT** ```" + msg + "```");
                                        }
                                        else
                                        {
                                            string failMessage = "invalid results for: ";
                                            if(res.stars == null)
                                            {
                                                failMessage += "stars";
                                            }
                                            if(res.attackType == null)
                                            {
                                                if (res.stars == null)
                                                    failMessage += ", ";
                                                failMessage += "attack type";
                                            }
                                            if (res.damage == null)
                                            {
                                                if (res.attackType == null)
                                                    failMessage += ", ";
                                                failMessage += "damege";
                                            }
                                            await e.Channel.SendMessage("**ENTER RESULT** ```ENTER RESULT FAILED: sorry " + user.UserName + " " + failMessage+ ".```");
                                        }

                                    }
                                    // if result already entered
                                    else
                                    {
                                        // validate stars attack type and damage
                                        AttackValidationResult res = ValidateResults(e.GetArg(1), e.GetArg(2), e.GetArg(3));
                                        if (res.success)
                                        {
                                            attack.Stars = res.stars;
                                            attack.AttackType = res.attackType;
                                            attack.Damage = res.damage;
                                            dc.SubmitChanges();
                                            await e.Channel.SendMessage("**ENTER RESULT** ```RESULT UPDATED: " + user.UserName + " actually hit #" + e.GetArg(0) + " for " + e.GetArg(1) + " stars and " + e.GetArg(3) + "% damage with " + e.GetArg(2) + ".```");
                                        }
                                        else
                                        {
                                            string failMessage = "invalid results for: ";
                                            if (res.stars == null)
                                            {
                                                failMessage += "stars";
                                            }
                                            if (res.attackType == null)
                                            {
                                                if (res.stars == null)
                                                    failMessage += ", ";
                                                failMessage += "attack type";
                                            }
                                            if (res.damage == null)
                                            {
                                                if (res.attackType == null || res.stars == null)
                                                    failMessage += ", ";
                                                failMessage += "damage";
                                            }
                                            await e.Channel.SendMessage("**ENTER RESULT** ```RESULT UPDATED FAILED: sorry " + user.UserName + " " + failMessage + ".```");
                                        }
                                    }
                                }
                                // if no claim or attack found for specified base
                                else
                                {
                                    await e.Channel.SendMessage("**ENTER RESULT** ```ENTER RESULT FAILED: sorry " + user.UserName + " you do not seem to have claimed or attacked that base.```");
                                }

                            }
                            // if user is not linked
                            else
                            {
                                await e.Channel.SendMessage("**ENTER RESULT** ```ENTER RESULT FAILED: account is not linked!```");
                            }
                        }
                        catch (Exception ex)
                        {
                            discord.Log.Error("RegisterDisplayWarInfoCommand", ex.Message);
                        }
                        #endregion
                    });
            });
        }

        public static AttackValidationResult ValidateResults(string stars, string attackType, string damage)
        {
            AttackValidationResult res = new AttackValidationResult();
            // check stars
            short numOfstars = -1;
            Int16.TryParse(stars, out numOfstars);
            if (numOfstars >= 0 && numOfstars <= 3)
                res.stars = numOfstars;
            else
                res.success = false;

            // check attack type
            ClasherDynastyDataContext dc = new ClasherDynastyDataContext();
            var attType = (from at in dc.AttackStrats
                           where at.AttackStrat1 == attackType
                           select at).FirstOrDefault();
            if (attType != null)
                res.attackType = attackType;
            else
                res.success = false;

            // check damage
            int dmgPerc = -1;
            Int32.TryParse(damage, out dmgPerc);
            if (dmgPerc >= 0 && dmgPerc <= 100)
            {
                res.damage = dmgPerc;
            }
            else
                res.success = false;

            // additional checks
            if (res.success)
            {
                //check 3 stars are 100% dmg
                if (res.stars == 3)
                {
                    if (res.damage != 100)
                    {
                        res.success = false;
                        res.damage = null;
                    }
                }

                //check 0 stars are less than 50% dmg
                if(res.stars == 0)
                {
                    if(res.damage >= 50)
                    {
                        res.success = false;
                        res.stars = null;
                        res.damage = null;
                    }
                }
            }

            return res;
        }

        public static void LogWarNews(string msg, DateTime eventTime, string userName)
        {

            ClasherDynastyDataContext dc = new ClasherDynastyDataContext();
            WarNew newNews = new WarNew();
            newNews.News = msg;
            newNews.PostDateTime = eventTime;
            newNews.UserName = userName;
            try
            {
                dc.WarNews.InsertOnSubmit(newNews);
                dc.SubmitChanges();
            }
            catch(Exception ex){ throw ex; }
          
        }
    }
   
}

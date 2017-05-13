using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClanBot.Commands
{
    static class EnemyNoteCommands
    {

        public static void RegisterEnemyNoteCommands(CommandService commands, DiscordClient discord)
        {
            commands.CreateGroup("enemy note", cgb =>
            {
                cgb.Category("=== ENEMY NOTES COMMANDS ===");
                cgb.CreateCommand("list")
                    .Description("!enemy note list - list all enemy base notes")
                    .Alias(new string[] { "-l"})
                    .AddCheck((command, user, channel) => !user.IsBot)
                    .Do(async (e) =>
                    {
                        #region enemy note list
                        try
                        {
                            ClasherDynastyDataContext dc = new ClasherDynastyDataContext();
                            List<EnemyBasesView> res = new List<EnemyBasesView>();
                            res = (from ebv in dc.EnemyBasesViews select ebv).ToList();

                            string message = "ENEMY BASE NOTES: \n";
                            foreach (EnemyBasesView eb in res)
                            {
                                if (eb.NOTES != null && eb.NOTES != "")
                                {
                                    message += "BASE: #" + eb.BASE__
                                                    + ((eb.CLAIMED_BY == null) ? " | NOT CLAIMED" : " | CLAIMED BY: " + eb.CLAIMED_BY)
                                                    + ((eb.ATTACKS == 0) ? "" : (" | ATTACKS: " + eb.ATTACKS
                                                    + " | BEST ATTACK: " + eb.BEST
                                                    + " | STARS: " + eb.STARS
                                                    + " | DAMAGE: " + eb.DAMAGE + "%"))
                                                    + ((eb.NOTES == null) ? "" : (" | NOTES: " + eb.NOTES)) + "\n";

                                }
                            }

                            await e.Channel.SendMessage(message);
                        }
                        catch (Exception ex) { discord.Log.Error("RegisterDisplayEnemyBaseNotes", ex.Message); }
                        #endregion
                    });

                cgb.CreateCommand("add")
                    .Description("!enemy note add [base number] ['note'] - add a note to the enemy war base.")
                    .Alias(new string[] { "-a" })
                    .AddCheck((command, user, channel) => !user.IsBot)
                    .Parameter("Base", ParameterType.Required)
                    .Parameter("Note", ParameterType.Required)
                    .Do(async (e) =>
                    {
                        #region enemy note add
                        try
                        {

                            ClasherDynastyDataContext dc = new ClasherDynastyDataContext();
                            War curWar = new War();
                            curWar = (from w in dc.Wars
                                      orderby w.Date descending
                                      select w).Take(1).SingleOrDefault();

                            EnemyBase eb = new EnemyBase();
                            eb = (from b in dc.EnemyBases
                                  where b.WarID == curWar.WarID
                                  && b.EnemyBaseNo.ToString() == e.GetArg("Base")
                                  select b).SingleOrDefault();


                            eb.Notes = e.GetArg("Note");

                            dc.SubmitChanges();

                            await e.Channel.SendMessage("NOTE ADDED TO ENEMY BASE #" + e.GetArg("Base"));
                        }
                        catch (Exception ex) { discord.Log.Error("RegisterAddBaseNoteCommand", ex.Message); }
                        #endregion
                    });

                cgb.CreateCommand("remove")
                    .Description("!enemy note remove [base number] - remove the note from the enemy war base.")
                    .Alias(new string[] { "-r" })
                    .AddCheck((command, user, channel) => !user.IsBot)
                    .Parameter("Base", ParameterType.Required)
                    .Do(async (e) =>
                    {
                        #region enemy note remove
                        try
                        {

                            ClasherDynastyDataContext dc = new ClasherDynastyDataContext();
                            War curWar = new War();
                            curWar = (from w in dc.Wars
                                      orderby w.Date descending
                                      select w).Take(1).SingleOrDefault();

                            EnemyBase eb = new EnemyBase();
                            eb = (from b in dc.EnemyBases
                                  where b.WarID == curWar.WarID
                                  && b.EnemyBaseNo.ToString() == e.GetArg("Base")
                                  select b).SingleOrDefault();


                            eb.Notes = null;

                            dc.SubmitChanges();

                            await e.Channel.SendMessage("NOTE REMOVED FROM ENEMY BASE #" + e.GetArg("Base"));
                        }
                        catch (Exception ex) { discord.Log.Error("RegisterRemoveBaseNoteCommand", ex.Message); }
                        #endregion
                    });
            });
        }
    }
}

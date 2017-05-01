using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClanBot
{
    static class EnemyBaseNotes
    {
        public static void RegisterDisplayEnemyBaseNotes(CommandService commands, DiscordClient discord)
        {
            commands.CreateCommand("Display Base Notes")
                .Description("!display base notes - show all enemy base notes")
                .AddCheck((command, user, channel) => !user.IsBot)
                .Do(async (e) =>
                {
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
                });
        }

        public static void RegisterAddBaseNoteCommand(CommandService commands, DiscordClient discord)
        {
            commands.CreateCommand("Add Enemy Note")
                .Description("!add enemy note [base number] ['note'] - add a note to the enemy war base.")
                .AddCheck((command, user, channel) => !user.IsBot)
                .Parameter("Base", ParameterType.Required)
                .Parameter("Note", ParameterType.Required)
                .Do(async (e) =>
                {
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
                });
        }

        public static void RegisterRemoveBaseNoteCommand(CommandService commands, DiscordClient discord)
        {
            commands.CreateCommand("Remove Enemy Note")
                .Description("!remove enemy note [base number] - remove the note from the enemy war base.")
                .AddCheck((command, user, channel) => !user.IsBot)
                .Parameter("Base", ParameterType.Required)
                .Do(async (e) =>
                {
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
                });
        }
    }
}

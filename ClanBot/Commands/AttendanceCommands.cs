using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClanBot.Commands
{
    static class AttendanceCommands
    {
        public static void RegisterAttendanceCommands(CommandService commands, DiscordClient discord)
        {
            commands.CreateGroup("attendance", cgb =>
            {
                cgb.Category("=== ATTENDANCE COMMANDS ===");
                cgb.CreateCommand("overall")
                    .Description("!attendance overall <'name'> - get members overall war attendace.")
                    .Alias(new string[] { "-o" })
                    .AddCheck((command, user, channel) => !user.IsBot)
                    .Parameter("Member", ParameterType.Optional)
                    .Do(async (e) =>
                    {
                        #region attendance overall
                        try
                        {
                            string player = string.Empty;

                            if (e.GetArg("Member") == null || e.GetArg("Member") == "")
                                player = e.User.Name;
                            else
                                player = e.GetArg("Member");

                            ClasherDynastyDataContext dc = new ClasherDynastyDataContext();
                            tvfGetAttendanceResult res = new tvfGetAttendanceResult();
                            res = (from att in dc.tvfGetAttendance(player) select att).FirstOrDefault();

                            await e.Channel.SendMessage(player.ToUpper() + "'S WAR ATTENDANCE: " + res.Attendance + "% (" + res.Attended + "/" + res.Totalwars + ")");
                        }
                        catch (Exception ex) { discord.Log.Error("RegisterGetPlayerAttendanceCommand", ex.Message); }
                        #endregion
                    });

                cgb.CreateCommand("season")
                    .Description("!attendance season <'name'> - get members current season war attendace.")
                    .Alias(new string[] { "-s" })
                    .AddCheck((command, user, channel) => !user.IsBot)
                    .Parameter("Member", ParameterType.Optional)
                    .Do(async (e) =>
                    {
                        #region attendance season
                        try
                        {
                            string player = string.Empty;

                            if (e.GetArg("Member") == null || e.GetArg("Member") == "")
                                player = e.User.Name;
                            else
                                player = e.GetArg("Member");

                            ClasherDynastyDataContext dc = new ClasherDynastyDataContext();
                            tvfGetCurSeasonAttendanceResult res = new tvfGetCurSeasonAttendanceResult();
                            res = (from att in dc.tvfGetCurSeasonAttendance(player) select att).FirstOrDefault();

                            await e.Channel.SendMessage(player.ToUpper() + "'S SEASON WAR ATTENDANCE: " + res.Attendance + "% (" + res.Attended + "/" + res.Totalwars + ")");
                        }
                        catch (Exception ex) { discord.Log.Error("RegisterGetPlayerAttendanceCommand", ex.Message); }
                        #endregion
                    });

                cgb.CreateCommand("top")
                    .Description("!attendance top <number of bases> - list the top N(default 10) war attenders.")
                    .Alias(new string[] { "-t" })
                    .AddCheck((command, user, channel) => !user.IsBot)
                    .Parameter("noOfBases", ParameterType.Optional)
                    .Do(async (e) =>
                    {
                        #region attendance top
                        try
                        {
                            int numberOfBases = 10;
                            string test = e.GetArg("noOfBases");

                            if (e.GetArg("noOfBases").Length >= 1)
                                int.TryParse(e.GetArg("noOfBases"), out numberOfBases);

                            ClasherDynastyDataContext dc = new ClasherDynastyDataContext();
                            List<ClanBot.tvfMembersAttendanceResult> members = (from att in dc.tvfMembersAttendance()
                                                                                select att).ToList();
                            members = members.OrderByDescending(m => m.CurrentSeason).ToList();

                            string message = "**CURRENT SEASON TOP WAR ATTENDERS**\n```";
                            int count = 0;
                            foreach (tvfMembersAttendanceResult m in members)
                            {
                                if (count >= numberOfBases)
                                    break;
                                message += "#" + (count + 1) + " - " + m.Username + " - " + m.CurrentSeasonStr + "% \n";
                                count++;
                            }
                            message += "```";

                            message += "**LAST SEASON TOP WAR ATTENDERS**\n```";
                            members = members.OrderByDescending(m => m.LastSeason).ToList();
                            count = 0;
                            foreach (tvfMembersAttendanceResult m in members)
                            {
                                if (count >= numberOfBases)
                                    break;
                                message += "#" + (count + 1) + " - " + m.Username + " - " + m.LastSeasonStr + "% \n";
                                count++;
                            }
                            message += "```";

                            message += "**OVERALL TOP WAR ATTENDERS**\n```";
                            members = members.OrderByDescending(m => m.Overall).ToList();
                            count = 0;
                            foreach (tvfMembersAttendanceResult m in members)
                            {
                                if (count >= numberOfBases)
                                    break;
                                message += "#" + (count + 1) + " - " + m.Username + " - " + m.OverallStr + "% \n";
                                count++;
                            }
                            message += "```";
                            await e.Channel.SendMessage(message);
                        }
                        catch (Exception ex) { discord.Log.Error("RegisterGetPlayerAttendanceCommand", ex.Message); }
                        #endregion
                    });
            });
        }
    }
}

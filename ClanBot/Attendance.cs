using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClanBot
{
    static class Attendance
    {
        public static void RegisterGetPlayerAttendanceCommand(CommandService commands, DiscordClient discord)
        {
            commands.CreateCommand("Attendance")
                .Description("!attendance <'name'> - get yours or another members war attendace.")
                .AddCheck((command, user, channel) => !user.IsBot)
                .Parameter("Member", ParameterType.Optional)
                .Do(async (e) =>
                {
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
                });
        }

        public static void RegisterGetPlayerCurSeasonAttendanceCommand(CommandService commands, DiscordClient discord)
        {
            commands.CreateCommand("Season Attendance")
                .Description("!season attendance <'name'> - get yours or another members war attendace for the current season.")
                .AddCheck((command, user, channel) => !user.IsBot)
                .Parameter("Member", ParameterType.Optional)
                .Do(async (e) =>
                {
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
                });
        }
    }
}

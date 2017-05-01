using ClanBot.ResultObjects;
using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClanBot
{
    static class Search
    {
        public static void RegisterSearchMembersCommand(CommandService commands, DiscordClient discord)
        {
            commands.CreateCommand("Search Members")
                .Description("!search members ['name'] - search for members/ex-members.")
                .AddCheck((command, user, channel) => !user.IsBot)
                .Parameter("Member", ParameterType.Required)
                .Do(async (e) =>
                {
                    try
                    {
                        ClasherDynastyDataContext dc = new ClasherDynastyDataContext();
                        List<MemberRank> res = new List<MemberRank>();
                        res = (from u in dc.Users
                               join ir in dc.UsersInRoles on u.UserId equals ir.UserId
                               join r in dc.Roles on ir.RoleId equals r.RoleId
                               where u.UserName.Contains(e.GetArg("Member"))
                               select new MemberRank { userName = u.UserName, userRole = r.RoleName }).ToList();

                        string message = "MEMBERS: \n";
                        foreach (MemberRank u in res)
                        {
                            if (u.userName != null && u.userName != "")
                            {
                                message += u.userName + "(" + u.userRole + ")\n";
                            }
                        }
                        if (message.Length <= 2000)
                            await e.Channel.SendMessage(message);
                        else
                            await e.Channel.SendMessage(res.Count() + " USERS FOUND... PLEASE REFINE MEMBER SEARCH!");
                    }
                    catch (Exception ex) { discord.Log.Error("RegisterSearchMembersCommand", ex.Message); }
                });
        }
    }
}

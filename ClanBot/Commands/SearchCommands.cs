using ClanBot.ResultObjects;
using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClanBot.Commands
{
    static class SearchCommands
    {
        public static void RegisterSearchCommands(CommandService commands, DiscordClient discord)
        {
            commands.CreateGroup("search", cgb =>
            {
                cgb.Category("=== SEARCH COMMANDS ===");
                cgb.CreateCommand("members")
                    .Description("!search members ['name'] - search for members/ex-members.")
                    .Alias(new string[] { "-m", "-mem", "member" })
                    .AddCheck((command, user, channel) => !user.IsBot)
                    .Parameter("Member", ParameterType.Required)
                    .Do(async (e) =>
                    {
                        #region search members
                        try
                        {
                            ClasherDynastyDataContext dc = new ClasherDynastyDataContext();
                            List<MemberRank> res = new List<MemberRank>();
                            res = (from u in dc.Users
                                   join ir in dc.UsersInRoles on u.UserId equals ir.UserId
                                   join r in dc.Roles on ir.RoleId equals r.RoleId
                                   where u.UserName.Contains(e.GetArg("Member"))
                                   select new MemberRank { userName = u.UserName, userRole = r.RoleName }).ToList();

                            if (res.Count < 1)
                            {
                                await e.Channel.SendMessage("NO USERS FOUND THAT MATCH YOUR SEARCH CRITERIA!");
                            }
                            else
                            {
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
                        }
                        catch (Exception ex) 
                        { 
                            discord.Log.Error("RegisterSearchMembersCommand", ex.Message); 
                        }
                        #endregion
                    });
            });
        }
    }
}

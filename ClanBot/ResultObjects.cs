using Discord;
using Discord.Commands;
using System;

namespace ClanBot.ResultObjects
{
    public class MemberRank
    {
        public string userName { get; set; }
        public string userRole { get; set; }
    }

    public class Command
    {
        public string command { get; set; }
        public string help { get; set; }
        public string result { get; set; }
    }

    public class MemberDetails
    {
        public int LEVEL { get; set; }
        public int TH { get; set; }
        public string NAME { get; set; }
        public string LOCATION { get; set; }
        public string ROLE { get; set; }
        public bool isEligble { get; set; }
        public int Attendance { get; set; }
        public int AttendanceOverall { get; set; }
        public string JOINED { get; set; }
        public int lastSeasonAttendance { get; set; }
    }

    public class RegisterNotification
    {
        public string user { get; set; }
        public DateTime lastNotificationTime { get; set; }
        public int intervalMin { get; set; }      
    }

    public class TwoPartCommand
    {
        public CommandEventArgs command { get; set; }
        public Channel channel { get; set; }
        public Discord.User user { get; set; }
    }
   
}

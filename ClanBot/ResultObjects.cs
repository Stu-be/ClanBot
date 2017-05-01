using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}

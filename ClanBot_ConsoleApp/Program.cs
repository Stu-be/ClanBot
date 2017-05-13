using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClanBot_ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("SERVER STARTING...");
            try
            {
                new ClanBot.ClasherDynBot();
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message + " - " + ex);
                Console.ReadLine();
            }
        }
    }
}

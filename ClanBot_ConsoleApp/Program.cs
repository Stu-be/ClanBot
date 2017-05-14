using System;

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

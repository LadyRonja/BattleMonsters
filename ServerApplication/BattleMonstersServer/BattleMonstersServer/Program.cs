using System;

namespace BattleMonstersServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Battle Monsters Server";

            Server.Start(50, 26950); //TODO: Get a random free port.

            Console.ReadKey();
        }
    }
}

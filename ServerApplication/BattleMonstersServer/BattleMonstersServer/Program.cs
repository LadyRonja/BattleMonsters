using System;
using System.Threading;

namespace BattleMonstersServer
{
    class Program
    {
        private static bool isRunning = false;

        //Server application begins execution here
        static void Main(string[] args)
        {
            Console.Title = "Battle Monsters Server";
            isRunning = true;

            //Run the method "MainThread()" as a delegate, allowing a way to run actions at a fixed interval.
            Thread mainThread = new Thread(new ThreadStart(MainThread));
            mainThread.Start();

            Server.Start(50, 26950); //TODO: Get a random free port.        
        }

        //While the server is running, execute everything within the nested while-loop at a fixed interval.
        //the thread sleeps until a set amount of real-time has passed in order eliviate CPU pressure
        private static void MainThread()
        {
            Console.WriteLine($"Main thread started. Running at {Constants.TICKS_PER_SEC} ticks per second.");
            DateTime _nextLoop = DateTime.Now;

            while (isRunning)
            {
                while (_nextLoop < DateTime.Now)
                {
                    //Everything in here is executed 30times/sec
                    GameLogic.Update(); 


                    //Do not execute as fast as possible, execute at fixed real-time intervals.
                    _nextLoop = _nextLoop.AddMilliseconds(Constants.MS_PER_SEC);
                    if (_nextLoop > DateTime.Now)
                    {
                        Thread.Sleep(_nextLoop - DateTime.Now);
                    }
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace BattleMonstersServer
{
    class GameLogic
    {

        //Runs operations at a fixed rate
        public static void Update() 
        {
            ThreadManager.UpdateMain();
        }
    }
}

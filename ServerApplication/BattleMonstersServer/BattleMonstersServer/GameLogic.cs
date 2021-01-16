using System;
using System.Collections.Generic;
using System.Text;

namespace BattleMonstersServer
{
    class GameLogic
    {
        public static void Update() 
        {
            ThreadManager.UpdateMain();
        }
    }
}

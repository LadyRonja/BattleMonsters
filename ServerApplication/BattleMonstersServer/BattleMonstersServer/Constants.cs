using System;
using System.Collections.Generic;
using System.Text;

namespace BattleMonstersServer
{
    class Constants
    {
        //Constants that allowes for the server to execute certain operations at a fixed rate.
        public const int TICKS_PER_SEC = 30;
        public const int MS_PER_SEC = 1000 / TICKS_PER_SEC;
    }
}

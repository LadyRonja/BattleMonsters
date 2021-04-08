using System;
using System.Collections.Generic;
using System.Text;

namespace BattleMonstersServer
{
    abstract class CardEffect
    {
        protected string description = "DEFAULT EFFECT DESCRIPTION";
        protected bool isDone = false;

        public string GetDescription()
        {
            return description;
        }

        public abstract void ExecuteEffect(Monster attacker, Monster defender);

        public bool IsDone() 
        {
            if (isDone)
            {
                isDone = false;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

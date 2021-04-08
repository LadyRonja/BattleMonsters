using System;
using System.Collections.Generic;
using System.Text;

namespace BattleMonstersServer
{
    class Monster
    {
        private ElementalType.ElementType myType = ElementalType.ElementType.SPECIAL;


        private int damageTokens = 0;
        private int slowingTokens = 0;

        
        //TODO: Damage calculation including type
        public void TakeDamage(int amount, ElementalType.ElementType type)
        {

        }

        public void AddDamageTokens(int amount) 
        {
            damageTokens += amount;
        }

        public void AddSlowingTokens(int amount)
        {
            slowingTokens += amount;
        }


        #region Getters/Setters
        internal ElementalType.ElementType MyType { get => myType; }


        #endregion
    }
}

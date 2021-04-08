using System;
using System.Collections.Generic;
using System.Text;

namespace BattleMonstersServer.CardEffects
{
    class ApplyDamageToken : CardEffect
    {

        private int tokensToApply = 0;
        
        //On creation set description
        public ApplyDamageToken(int amount)
        {
            tokensToApply = amount;
            description = $"Apply {amount} damaging tokens to the active enemy monster.";

        }

        public override void ExecuteEffect(Monster attacker, Monster defender)
        {
            defender.AddDamageTokens(tokensToApply);
            isDone = true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace BattleMonstersServer.CardEffects
{
    class DealDamage : CardEffect
    {
        private int baseDamage;
        private ElementalType.ElementType damageType = ElementalType.ElementType.SPECIAL;

        public DealDamage(int baseAmount, ElementalType.ElementType type)
        {
            baseDamage = baseAmount;
            damageType = type;
            description = $"Deal {baseAmount} {damageType.ToString()} damage";
        }

        public override void ExecuteEffect(Monster attacker, Monster defender)
        {
            int stabMultiplier = 1;

            if (attacker.MyType == damageType)
            {
                stabMultiplier = 2;
            }

            defender.TakeDamage(baseDamage * stabMultiplier, damageType);
        }
    }
}

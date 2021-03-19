using System;
using System.Collections.Generic;
using System.Text;

namespace BattleMonstersServer
{
    class CardFactory
    {
        public Card[] allCards = new Card[17];

        #region Singleton
        //TODO: Make threadsafe
        private static CardFactory Instance = null;
        private CardFactory() 
        { 
            
        }

        public static CardFactory GetInstance()
        {
            if (Instance == null)
            {
                Instance = new CardFactory();
            }

            return Instance;
        }
        #endregion

        //Create all 17 cards and add the to the allCard array
        //Could be improved by reading card info from an exel sheet
        //TODO: Add effects
        private void CreateAllCards() 
        {
            int arrayPos = 0;
            List<CardEffect> tempEffectList = new List<CardEffect>();

            #region Rotators
            //The basic rotator cards

            #region Rotate Left
            //tempEffectList.add(Rotate(Left));
            allCards[arrayPos] = new Card("Rotate Left", ElementalType.ElementType.ROTATOR, 0, tempEffectList);

            tempEffectList.Clear();
            arrayPos++;
            #endregion

            #region Rotate Right
            //tempEffectList.add(Rotate(Right));
            allCards[arrayPos] = new Card("Rotate Right", ElementalType.ElementType.ROTATOR, 0, tempEffectList);

            tempEffectList.Clear();
            arrayPos++;
            #endregion

            #endregion

            #region SpecialCards
            //Cards of the special class

            #region Heealing Meditation
            //tempEffectList.add(Heal(5));
            allCards[arrayPos] = new Card("Healing Meditation", ElementalType.ElementType.SPECIAL, 2, tempEffectList);

            tempEffectList.Clear();
            arrayPos++;
            #endregion

            #region Infect Wounds
            //tempEffectList.add(Priority(Case(OpponentTypePlayed(Rotator))));
            //tempEffectList.add(DamageToken(3))
            allCards[arrayPos] = new Card("Infect Wounds", ElementalType.ElementType.SPECIAL, 6, tempEffectList);

            tempEffectList.Clear();
            arrayPos++;
            #endregion

            #region Damnation of Passivity
            //tempEffectList.add(SlowToken(Inactive(Enemy(1)));
            //tempEffectList.add(DamageToken(Inactive(All(1)));
            allCards[arrayPos] = new Card("Damnation of Passivity", ElementalType.ElementType.SPECIAL, 1, tempEffectList);

            tempEffectList.Clear();
            arrayPos++;
            #endregion
            #endregion

            #region FireCards

            #region CardName

            arrayPos++;
            #endregion


            #endregion

            #region WaterCards

            #region CardName

            arrayPos++;
            #endregion


            #endregion

            #region GrassCards

            #region CardName

            arrayPos++;
            #endregion


            #endregion
        }
    }
}

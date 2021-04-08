using BattleMonstersServer.CardEffects;
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
            CreateAllCards();
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
            tempEffectList.Add(new ApplyDamageToken(3));
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

            #region Fiery Tackle
            tempEffectList.Add(new DealDamage(2, ElementalType.ElementType.FIRE));
            allCards[arrayPos] = new Card("Fiery Tackle", ElementalType.ElementType.FIRE, 3, tempEffectList);

            tempEffectList.Clear();
            arrayPos++;
            #endregion

            #region Flash Fire
            tempEffectList.Add(new DealDamage(1, ElementalType.ElementType.FIRE));
            //tempEffectList.add(SlowToken(Self(1)));
            //tempEffectList.add(Rotate());
            allCards[arrayPos] = new Card("Flash Fire", ElementalType.ElementType.FIRE, 2, tempEffectList);

            tempEffectList.Clear();
            arrayPos++;
            #endregion

            #region Burn Weakness
            //tempEffectList.add(BurnTokensDamage(Own(All)));
            allCards[arrayPos] = new Card("Burn Weakness", ElementalType.ElementType.FIRE, 8, tempEffectList);

            tempEffectList.Clear();
            arrayPos++;
            #endregion

            #region Blazing Inferno
            tempEffectList.Add(new DealDamage(3, ElementalType.ElementType.FIRE));
            //tempEffectList.add(Damage(Self(3)));
            allCards[arrayPos] = new Card("Blazing Inferno", ElementalType.ElementType.FIRE, 7, tempEffectList);

            tempEffectList.Clear();
            arrayPos++;
            #endregion

            #endregion

            #region WaterCards

            #region Hydro Beam
            tempEffectList.Add(new DealDamage(2, ElementalType.ElementType.WATER));
            allCards[arrayPos] = new Card("Hydro Beam", ElementalType.ElementType.WATER, 3, tempEffectList);

            tempEffectList.Clear();
            arrayPos++;
            #endregion

            #region Tidal Wave
            tempEffectList.Add(new DealDamage(2, ElementalType.ElementType.WATER));
            //tempEffectList.add(Rotate());
            allCards[arrayPos] = new Card("Tidal Wave", ElementalType.ElementType.WATER, 6, tempEffectList);

            tempEffectList.Clear();
            arrayPos++;
            #endregion

            #region Toxic Waters
            tempEffectList.Add(new DealDamage(2, ElementalType.ElementType.WATER));
            //tempEffectList.add(DamageToken(Random(ToxicWaters())));
            allCards[arrayPos] = new Card("Toxic Waters", ElementalType.ElementType.WATER, 7, tempEffectList);

            tempEffectList.Clear();
            arrayPos++;
            #endregion

            #region Wet Mud
            //tempEffectList.add(DamageImmunity());
            //tempEffectList.add(SlowToken(3))
            allCards[arrayPos] = new Card("Flash Fire", ElementalType.ElementType.WATER, 4, tempEffectList);

            tempEffectList.Clear();
            arrayPos++;
            #endregion

            #endregion

            #region GrassCards

            #region Thorny Whip
            tempEffectList.Add(new DealDamage(2, ElementalType.ElementType.GRASS));
            allCards[arrayPos] = new Card("Thorny Whip", ElementalType.ElementType.GRASS, 3, tempEffectList);

            tempEffectList.Clear();
            arrayPos++;
            #endregion

            #region Uproot
            //tempEffectList.add(Rotate(Enemy(Random(Uproot()))));
            allCards[arrayPos] = new Card("Uproot", ElementalType.ElementType.GRASS, 2, tempEffectList);

            tempEffectList.Clear();
            arrayPos++;
            #endregion

            #region Strangling Vine
            tempEffectList.Add(new DealDamage(2, ElementalType.ElementType.GRASS));
            //tempEffectList.add(Rotate(Lock()));
            allCards[arrayPos] = new Card("Strangling Vine", ElementalType.ElementType.GRASS, 2, tempEffectList);

            tempEffectList.Clear();
            arrayPos++;
            #endregion

            #region Might of Nature
            tempEffectList.Add(new DealDamage(4, ElementalType.ElementType.GRASS));
            allCards[arrayPos] = new Card("Might of Nature", ElementalType.ElementType.GRASS, 9, tempEffectList);

            tempEffectList.Clear();
            arrayPos++;
            #endregion

            #endregion

            ListAllCards();
        }

        //Debug function, check card info on serverside.
        public void ListAllCards()
        {
            Console.WriteLine("Listing all existing cards on server:");

            for (int i = 0; i < allCards.Length; i++)
            {

                Console.WriteLine(allCards[i].Name + ", "  + allCards[i].Initative + ", " + allCards[i].MyType.ToString() + ", " + allCards[i].CardText);
            }

            Console.WriteLine("All cards on server listed.");
        }
    }
}

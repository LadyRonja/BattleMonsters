using System;
using System.Collections.Generic;
using System.Text;

namespace BattleMonstersServer
{
    class Card
    {
        private static int ID_COUNTER = 0;

        private string name = "Unnamed Card";
        private int id = -1;
        private int initative = -1;
        private ElementalType.ElementType myType;
        private List<CardEffect> cardEffects;

        private string cardText = "";

        public Card(string _name, ElementalType.ElementType _type, int _initiative, List<CardEffect> _effects)
        {
            //Each card needs a unique ID, up counter and assign the new ID
            //TODO: Add a check to ensure no ID duplication
            //TODO: Add a failsafe
            ID_COUNTER++;
            id = ID_COUNTER;

            //Card data
            name = _name;
            initative = _initiative;
            myType = _type;
            cardEffects = _effects;

            //Display all card effect descriptions on the card text.
            for (int i = 0; i < cardEffects.Count; i++)
            {
                cardText += cardEffects[i].GetDescription();
                cardText += "\n"; //line break for each effect
            }
        }
    }
}

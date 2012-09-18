using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerAI
{
    enum Suit
    {
        SPADES = 0, HEARTS = 1, CLOVES = 2, DIAMONDS = 3
    }
    
    class Card
    {
        public Suit Suit;
        public int Value;

        public Card(Suit suit, int value)
        {
            Suit = suit;
            Value = value;
        }

        public string ToString()
        {
            string s = "";
            switch (Value)
            {
                case 11:
                    s += "J";
                    break;
                case 12:
                    s += "Q";
                    break;
                case 13:
                    s += "K";
                    break;
                case 14:
                    s += "A";
                    break;
                default:
                    s += Value;
                    break;
            }
            switch (Suit)
            {
                case PokerAI.Suit.CLOVES:
                    s += "♣";
                    break;
                case PokerAI.Suit.DIAMONDS:
                    s += "♦";
                    break;
                case PokerAI.Suit.HEARTS:
                    s += "♥";
                    break;
                case PokerAI.Suit.SPADES:
                    s += "♠";
                    break;
            }
            return " " + s + " ";
        }
    }

    class Deck
    {
        Stack<Card> cards;

        public Deck(List<Card> hand = null)
        {
            cards = new Stack<Card>();
            Shuffle(hand);
        }

        public void Shuffle(List<Card> holeCards = null)
        {
            List<Card> temp = new List<Card>();
            for (int i = 2; i < 15; i++)
            {
                temp.Add(new Card(Suit.SPADES, i));
                temp.Add(new Card(Suit.HEARTS, i));
                temp.Add(new Card(Suit.CLOVES, i));
                temp.Add(new Card(Suit.DIAMONDS, i));
            }
            if(holeCards != null)
            {
                temp = temp.Where(c => 
                    (c.Value != holeCards[0].Value || c.Suit != holeCards[0].Suit)
                    && (c.Value != holeCards[1].Value || c.Suit != holeCards[1].Suit)).ToList();
            }

            Random random = new Random();
            int randomNumber = 0;
            cards.Clear();
            while (temp.Count > 0)
            {
                randomNumber = random.Next(temp.Count);
                cards.Push(temp[randomNumber]);
                temp.RemoveAt(randomNumber);
            }
        }

        public Card DrawCard()
        {
            return cards.Pop();
        }

        public int Size()
        {
            return cards.Count;
        }
    }
}

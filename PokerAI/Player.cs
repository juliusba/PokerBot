using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerAI
{
    class Player
    {
        private readonly int initMoney = 1000;
        private Table Table;

        private bool _folded;
        public bool Folded { 
            get { return _folded; }
            private set { 
                _folded = value;
                CanPlay = !value && Stack != 0;
            }
        }

        private int _stack;
        public int Stack
        {
            get { return _stack; }
            set
            {
                if (value >= 0) _stack = value;
                else return;
                CanPlay = !_folded && value != 0;
            }
        }

        private int _myCurrentBet;
        public int MyCurrentBet
        {
            get { return _myCurrentBet; }
            private set
            {
                if(value - _myCurrentBet > 0) Stack -= value - _myCurrentBet;
                _myCurrentBet = value;
            }
        }

        private int _sidePot;
        public int SidePot
        {
            get { return _sidePot; }
            set
            {
                if (value >= -1) _sidePot = value;
                else return;
            }
        }

        public PowerRating PowerRating { get; private set; }

        public Context Context;
        public PlayerModel Model;

        public List<Card> Hand;
        public bool CanPlay { get; private set; }

        public string name;
        private int type;

        public Player(Table table, string name, int type)
        {
            Table = table;
            Stack = initMoney;
            Hand = new List<Card>();

            this.name = name;
            this.type = type;
        }

        public void PrepareHand()
        {
            _myCurrentBet = 0;
            SidePot = -1;
            if (Stack == 0) Folded = true;
            else Folded = false;

            Console.WriteLine(name + " has: " + Stack + " dollars");
        }

        public Action Action()
        {
            switch (type)
            {
                case 1:
                    return phaseOneAction(Table.getPot(), Table.getCurrentBet(), Table.getPlayersLeft(), Table.getCommunityCards());
                case 2:
                    return phaseTwoAction(Table.getPot(), Table.getCurrentBet(), Table.getPlayersLeft(), Table.getCommunityCards());
                case 3:
                    return phaseThreeAction(Table.getPot(), Table.getCurrentBet(), Table.getPlayersLeft(), Table.getCommunityCards());
                default:
                    return phaseOneAction(Table.getPot(), Table.getCurrentBet(), Table.getPlayersLeft(), Table.getCommunityCards());
            }
            
        }

        private Action phaseOneAction(int pot, int currentBet, int playersLeft, List<Card> communityCards)
        {
            Action action;
            updatePowerRating(communityCards);

            if (currentBet != MyCurrentBet)
            {
                int callAmount = currentBet - MyCurrentBet;
                if (callAmount > Stack)
                {
                    action = new Action(ActionType.ALLIN, Stack, 0);
                    MyCurrentBet += Stack;
                }
                else if (PowerRating.first == 8)
                {
                    action = new Action(ActionType.ALLIN, callAmount, Stack - callAmount);
                    MyCurrentBet += Stack;
                }
                else if (PowerRating.first > 2)
                {
                    int bet = PowerRating.first * (initMoney / 200);
                    if (callAmount + bet >= Stack)
                    {
                        action = new Action(ActionType.ALLIN, callAmount, Stack - callAmount);
                        MyCurrentBet += Stack;
                    }
                    else
                    {
                        action = new Action(ActionType.BET, callAmount, bet);
                        MyCurrentBet += callAmount + bet;
                    }
                }
                else
                {
                    Folded = true;
                    return new Action(ActionType.FOLD, 0, 0);
                }
            }
            else
            {
                return null;
            }
            return action;
        }

        private Action phaseTwoAction(int pot, int currentBet, int playersLeft, List<Card> communityCards)
        {
            double handStrength = calculateHandStrength(playersLeft, communityCards);


            return null;
        }

        private Action phaseThreeAction(int pot, int currentBet, int playersLeft, List<Card> communityCards)
        {
            return null;
        }

        public int PutBlind(int blind)
        {
            if (Stack > blind)
            {
                MyCurrentBet = blind;
            }
            else
            {
                MyCurrentBet = Stack;
            }

            Console.WriteLine(name + " placed blind of " + MyCurrentBet + " dollar");
            return MyCurrentBet;
        }

        public void ActionMade(Action action, Player player)
        {

        }

        public void updatePowerRating(List<Card> communityCards)
        {
            List<Card> temp = new List<Card>();
            temp.AddRange(Hand);
            temp.AddRange(communityCards);
            PowerRating = new PowerRating(temp);
        }

        private double calculateHandStrength(int playersLeft, List<Card> communityCards)
        {
            if (communityCards.Count == 0)
            {
                return MainClass.getWins(Hand, playersLeft);
            }
            else
            {
                int wins = 0;
                int draws = 0;
                int losses = 0;

                List<Card> myHand = new List<Card>();
                myHand.AddRange(Hand);
                myHand.AddRange(Table.getCommunityCards());
                List<Card> temp = new List<Card>();
                temp.AddRange(myHand);
                Deck deckOne = new Deck(temp);
                Card cardOne;
                Deck deckTwo;
                Card cardTwo;
                List<Card> otherHand = new List<Card>();
                otherHand.AddRange(Table.getCommunityCards());
                PowerRating myPowerRating;
                PowerRating otherPowerRating;
                while (deckOne.Size() > 0)
                {
                    cardOne = deckOne.DrawCard();
                    otherHand.Add(cardOne);
                    temp.Add(cardOne);
                    deckTwo = new Deck(temp);
                    while (deckTwo.Size() > 0)
                    {
                        cardTwo = deckTwo.DrawCard();
                        otherHand.Add(cardTwo);

                        if (otherHand.Count == 5)
                        {
                            temp.Add(cardTwo);
                            Deck deckThree = new Deck(temp);
                            while (deckThree.Size() > 0)
                            {
                                Card cardThree = deckThree.DrawCard();
                                myHand.Add(cardThree);
                                otherHand.Add(cardThree);

                                temp.Add(cardThree);
                                Deck deckFour = new Deck(temp);
                                while (deckFour.Size() > 0)
                                {
                                    Card cardFour = deckFour.DrawCard();
                                    myHand.Add(cardFour);
                                    otherHand.Add(cardFour);

                                    myPowerRating = new PowerRating(myHand);
                                    otherPowerRating = new PowerRating(otherHand);

                                    switch (myPowerRating.betterThan(otherPowerRating))
                                    {
                                        case 1:
                                            wins++;
                                            break;
                                        case 0:
                                            draws++;
                                            break;
                                        case -1:
                                            losses++;
                                            break;
                                    }

                                    myHand.Remove(cardFour);
                                    otherHand.Remove(cardFour);
                                }

                                temp.Remove(cardThree);

                                myHand.Remove(cardThree);
                                otherHand.Remove(cardThree);
                            }
                            temp.Remove(cardTwo);
                        }
                        else if (otherHand.Count == 6)
                        {
                            temp.Add(cardTwo);
                            Deck deckThree = new Deck(temp);
                            while (deckThree.Size() > 0)
                            {
                                Card cardThree = deckThree.DrawCard();
                                myHand.Add(cardThree);
                                otherHand.Add(cardThree);

                                myPowerRating = new PowerRating(myHand);
                                otherPowerRating = new PowerRating(otherHand);

                                switch (myPowerRating.betterThan(otherPowerRating))
                                {
                                    case 1:
                                        wins++;
                                        break;
                                    case 0:
                                        draws++;
                                        break;
                                    case -1:
                                        losses++;
                                        break;
                                }

                                myHand.Remove(cardThree);
                                otherHand.Remove(cardThree);
                            }
                            temp.Remove(cardTwo);
                        }
                        else
                        {
                            myPowerRating = new PowerRating(myHand);
                            otherPowerRating = new PowerRating(otherHand);

                            switch (myPowerRating.betterThan(otherPowerRating))
                            {
                                case 1:
                                    wins++;
                                    break;
                                case 0:
                                    draws++;
                                    break;
                                case -1:
                                    losses++;
                                    break;
                            }
                        }

                        otherHand.Remove(cardTwo);
                    }
                    otherHand.Remove(cardOne);
                    temp.Remove(cardOne);
                }

                double strength = wins + (draws / 2);
                strength = strength / (wins + draws + losses);
                strength = Math.Pow(strength, playersLeft);
                return strength;
            }
        }
    }

    class Action
    {
        public ActionType type;
        public int callAmount;
        public int betAmount;

        public Action(ActionType type, int callAmount, int betAmount){
            this.type = type;
            this.callAmount = callAmount;
            this.betAmount = betAmount;

        }
    }

    enum ActionType {FOLD, CHECK, CALL, BET, RAISE, RERAISE, ALLIN}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerAI
{
    class PlayerModel
    {
        //public double estimatedHandStrength;
        //public double estimatedAggressiveness;
        //public double estimatedBluffRate;

        public Dictionary<Context, List<Tuple<Action, double>>> Context_ActionProbabilityPairs = new Dictionary<Context,List<Tuple<Action,double>>>();
        public Dictionary<Tuple<Context, Action>, double> ContextAction_EstimatedHandStrengthPairs = new Dictionary<Tuple<Context,Action>,double>()
            ;

        private List<Tuple<Context, Action>> temp = new List<Tuple<Context,Action>>();

        public void ActionMadeFromContext(Context context, Action currentAction)
        {
            //throw new NotImplementedException();
        }

        public void CardsShown(List<Card> list, List<Card> communityCards)
        {
            //throw new NotImplementedException();
            // Flytt fra temp til faktiske lister...
        }
    }

    class Context
    {
        private int pot;
        public int PlayerCount { get; private set; }
        public int HandStep; //Preflop, flop, turn, river...

        private Ratio _stack_PotRatio;
        public double Stack_PotRatio
        {
            get { return (int)_stack_PotRatio / 10; }
            private set
            {
                _stack_PotRatio = (Ratio) Math.Round(value * 10);
            }
        }

        private Ratio _toCall_PotRatio;
        public double ToCall_PotRatio
        {
            get
            {
                return (int)_toCall_PotRatio / 5;
            }
            set
            {
                _toCall_PotRatio = (Ratio)Math.Round(value * 5);
            }
        }

        private Ratio _checkCount_PlayerCountRatio;
        public double CheckCount_PlayerCountRatio
        {
            get { return (int)_checkCount_PlayerCountRatio / 5; }
            private set
            {
                _checkCount_PlayerCountRatio = (Ratio)Math.Round(value * 5);
            }
        }

        private Ratio _betRaiseCount_PlayerCountRatio;
        public double BetRaiseCount_PlayerCountRatio
        {
            get { return (int)_betRaiseCount_PlayerCountRatio / 10; }
            private set
            {
                _betRaiseCount_PlayerCountRatio = (Ratio)Math.Round(value * 10);
            }
        }

        public bool MadeBet;

        public double EstimatedHandStrength; // the strength he thinks i think he has.

        public Context(int playerCount)
        {
            this.PlayerCount = playerCount;
        }

        public void UpdateBeforeAction(int pot, int stack, int toCall)
        {
            this.pot = pot;
            this.Stack_PotRatio = stack / pot;
            this.ToCall_PotRatio = toCall / pot;
        }

        public void UpdateAfterAction(bool bet)
        {
            MadeBet = bet;
        }

        public void BetRaise()
        {
            BetRaiseCount_PlayerCountRatio = (BetRaiseCount_PlayerCountRatio * PlayerCount + 1) / PlayerCount;
        }

        public void Check()
        {
            CheckCount_PlayerCountRatio = (CheckCount_PlayerCountRatio * PlayerCount + 1) / PlayerCount;
        }

        public void Fold()
        {
            PlayerCount--;
        }

        //private Ratio _playersCurrentBet_PotRatio;
        //public double PlayersCurrentBet_PotRatio
        //{
        //    get
        //    {
        //        return (int)_stack_PotRatio / 10;
        //    }
        //    set
        //    {
        //        _stack_PotRatio = (Ratio)Math.Round(value * 10);
        //    }
        //}
        
        

        //private Ratio _callCount_PlayerCountRatio;
        //public double CallCount_PlayerCountRatio
        //{
        //    get
        //    {
        //        return (int)_callCount_PlayerCountRatio / 8;
        //    }
        //    set
        //    {
        //        _callCount_PlayerCountRatio = (Ratio)Math.Round(value * 8);
        //    }
        //}
    }

    enum Ratio
    {
        VERY_SMALL = 1, SMALL = 2, MEDIUM = 3, BIG = 4, VERY_BIG = 5
    }

    //enum Quantity
    //{
    //    NONE = 0, ONCE = 1, MORE_THAN_ONCE = 2
    //}
}

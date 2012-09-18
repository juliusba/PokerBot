using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerAI
{
    class Table
    {
        private readonly int _roundCount = 100;

        private List<Player> players;
        private Deck deck;
        private List<Card> communityCards;
        private int _pot;
        private int pot
        {
            get { return _pot; }
            set
            {
                if (value >= 0) _pot = value;
                else return;
            }
        }

        private int blind;
        private int currentRoundNumber;
        private int currentBet;

        private readonly List<string> playerNames = new List<string>()
        {
            "Julius      ",
            "Martin      ",
            "Dan Herman  ",
            "Isak        ",
            "Leif        ",
            "Gustav      ",
            "Nils        ",
            "Lars        ",
            "Ole         ",
            "Ulf         "
        };

        public Table(int numberOfPlayers)
        {
            numberOfPlayers = Math.Min(22, numberOfPlayers);
            players = new List<Player>();

            for (int i = 0; i < numberOfPlayers; i++)
            {
                if (i < 3)
                    players.Add(new Player(this, playerNames[i], 1));
                else
                    players.Add(new Player(this, playerNames[i], 1));
                    //players.Add(new Player(this, playerNames[i], 2));
            }

            deck = new Deck();
            communityCards = new List<Card>();
            currentRoundNumber = 0;
        }

        public void Play()
        {
            for (currentRoundNumber = 0; currentRoundNumber < _roundCount; currentRoundNumber++)
            {   
                if (currentRoundNumber % 10 == 0) blind += 10;

                List<Player> winners;

                if (!prepareHand())
                {
                    Console.WriteLine("Game Over! " + players[0].name + " is the winner!");
                    return;
                }
                Console.WriteLine("New round was prepared!");

                Console.WriteLine("Starting preflop bets...");
                doBets();
                winners = players.Where(p => !p.Folded).ToList();
                if (winners.Count == 1)
                {
                    endRound(winners.First());
                    continue;
                }

                string output = "Flop: ";
                communityCards.Add(deck.DrawCard());
                communityCards.Add(deck.DrawCard());
                communityCards.Add(deck.DrawCard());
                foreach (Card card in communityCards)
                    output += card.ToString();
                Console.WriteLine(output);
                currentBet = 0;
                doBets();
                winners = players.Where(p => !p.Folded).ToList();
                if (winners.Count == 1)
                {
                    endRound(winners.First());
                    continue;
                }

                output = "Turn: ";
                communityCards.Add(deck.DrawCard());
                foreach (Card card in communityCards)
                    output += card.ToString();
                Console.WriteLine(output);
                currentBet = 0;
                doBets();
                winners = players.Where(p => !p.Folded).ToList();
                if (winners.Count == 1)
                {
                    endRound(winners.First());
                    continue;
                }

                output = "River: ";
                communityCards.Add(deck.DrawCard());
                foreach (Card card in communityCards)
                    output += card.ToString();
                Console.WriteLine(output);
                currentBet = 0;
                doBets();
                winners = players.Where(p => !p.Folded).ToList();
                if (winners.Count == 1)
                {
                    endRound(winners.First());
                    continue;
                }
                else
                {
                    List<List<Player>> winnerGroupings = showCards(winners);
                    endRound(winnerGroupings);
                }
            }
        }

        private bool prepareHand()
        {
            pot = 0;

            int i = 0;
            while (i < players.Count)
            {
                if (players[i].Stack == 0)
                    players.RemoveAt(i);
                else i++;
            }
            if (players.Count == 1) return false;

            Player dealer = players[players.Count - 1];
            players.RemoveAt(players.Count - 1);
            players.Insert(0, dealer);
            
            deck.Shuffle();
            communityCards.Clear();

            foreach (Player p in players)
            {
                p.Hand.Clear();
                p.Hand.Add(deck.DrawCard());
                p.Hand.Add(deck.DrawCard());
                p.PrepareHand();
            }

            pot =  players[players.Count - 2].PutBlind(blind);
            pot += players[players.Count - 1].PutBlind(blind * 2);

            currentBet = blind * 2;

            return true;
        }

        private void doBets()
        {
            Action currentAction;
            string output;

            List<Player> canPlay = players.Where(p => !p.Folded && !(p.Stack == 0)).ToList();

            int canPlayCount = canPlay.Count;
            foreach (Player p in players) p.updatePowerRating(communityCards);
            while (canPlayCount > 1)
            {
                foreach (Player p in canPlay)
                {
                    p.Context.UpdateBeforeAction(pot, p.Stack, currentBet - p.MyCurrentBet);
                    currentAction = p.Action();
                    p.Model.ActionMadeFromContext(p.Context, currentAction);
                    p.Context.UpdateAfterAction(currentAction.betAmount != 0);

                    if (currentAction == null) return;
                    currentBet += currentAction.betAmount;
                    pot += currentAction.callAmount + currentAction.betAmount;

                    output = p.name + " ";
                    switch (currentAction.type)
                    {
                        case ActionType.FOLD:
                            output += "FOLD";
                            break;
                        case ActionType.CHECK:
                            output += "CHECK";
                            break;
                        case ActionType.CALL:
                            output += "CALL(" + currentAction.callAmount + ")";
                            break;
                        case ActionType.BET:
                            output += "BET(" + currentAction.callAmount + " + " + currentAction.betAmount + ")";
                            break;
                        case ActionType.RAISE:
                            output += "RAISE(" + currentAction.callAmount + " + " + currentAction.betAmount + ")";
                            break;
                        case ActionType.RERAISE:
                            output += "called (" + currentAction.callAmount + "), and reraised the bet with " + currentAction.betAmount + "...";
                            break;
                        case ActionType.ALLIN:
                            p.SidePot = pot;
                            if (currentBet > p.MyCurrentBet)
                                foreach (Player p2 in players)
                                    if (p2.MyCurrentBet > p.MyCurrentBet)
                                        p.SidePot -= p2.MyCurrentBet - p.MyCurrentBet; 
                            output += "ALL IN(" + currentAction.callAmount + " + " + currentAction.betAmount + ")";
                            break;
                    }
                    Console.WriteLine(output + " POT: " + pot);
                    
                    if (currentAction.type == ActionType.FOLD && canPlayCount == 2)
                        break;
                    
                    if(currentAction.betAmount != 0) foreach(Player p2 in players) p2.Context.BetRaise();
                    else if(currentAction.callAmount == 0) foreach(Player p2 in players) p2.Context.Check();

                    for(int i = 0; i < players.Count; i++)
                    {
                        if (players[i].SidePot != -1 && players[i] != p)
                        {
                            int differanse = players[i].MyCurrentBet - (p.MyCurrentBet - (currentAction.callAmount + currentAction.betAmount));
                            if (differanse > 0) players[i].SidePot += Math.Min(differanse, (currentAction.callAmount + currentAction.betAmount)); 
                        }
                    }
                }

                foreach (Player p in players)
                {
                    if (!p.CanPlay)
                    {
                        if (canPlay.Remove(p))
                            canPlayCount--;
                    }
                }
            }
        }

        private List<List<Player>> showCards(List<Player> remainingPlayers)
        {
            foreach (Player p in remainingPlayers) p.Model.CardsShown(p.Hand, communityCards);

            List<IGrouping<PowerRating, Player>> tempWinners = remainingPlayers.GroupBy(p => p.PowerRating).ToList();
            if (tempWinners.Count == 1) return tempWinners.Select(g => g.ToList()).ToList();
            else
            {
                List<IGrouping<PowerRating, Player>> winners = new List<IGrouping<PowerRating, Player>>();
                winners.Add(tempWinners[0]);
                for (int i = 1; i < tempWinners.Count; i++)
                {
                    for (int j = 0; j < winners.Count; j++)
                    {
                        if(tempWinners[i].Key.betterThan(winners[j].Key) == 1)
                        {
                            winners.Insert(j, tempWinners[i]);
                            break;
                        }
                    }
                    if (!winners.Contains(tempWinners[i])) winners.Add(tempWinners[i]);
                }
                List<List<Player>> winnerLists = winners.Select(g => g.ToList()).ToList();
                foreach (List<Player> players in winnerLists)
                {
                    foreach (Player p in players)
                    {
                        Console.WriteLine(p.name + " had: " + p.Hand[0].ToString() + p.Hand[1].ToString() + " " + p.PowerRating.ToString());
                    }
                }
                return winnerLists;
            }
        }

        private void endRound(Player winner)
        {
            winner.Stack += pot;
            Console.WriteLine(winner.name + " won " + pot);
            Console.WriteLine("HAND " + currentRoundNumber + " OVER");
        }

        private void endRound(List<List<Player>> winners)
        {
            int win;
            foreach (List<Player> winnerGroup in winners)
            {
                List<Player> orderedWinnerGroup = winnerGroup.OrderBy(wg => wg.MyCurrentBet).ToList();
                foreach(Player winner in winnerGroup)
                {
                    if (winner.SidePot != -1)
                        win = winner.SidePot / winnerGroup.Count;
                    else
                        win = pot / winnerGroup.Count;

                    winner.Stack += win;
                    pot -= win;
                    foreach (Player p in players)
                        if (p.SidePot != -1) 
                            p.SidePot = Math.Max(0, p.SidePot - win);

                    if(win > 0) 
                        Console.WriteLine(winner.name + " won " + win);
                    else 
                        Console.WriteLine(winner.name + " lost his " + winner.MyCurrentBet);
                }
            }

            Console.WriteLine("HAND " + currentRoundNumber + " OVER");
        }

        public List<Card> getCommunityCards()
        {
            return communityCards;
        }

        public int getPlayersLeft()
        {
            int playersLeft = 0;
            foreach (Player p in players) if (!p.Folded) playersLeft++;
            return playersLeft;
        }

        public int getCurrentBet()
        {
            return currentBet;
        }

        public int getPot()
        {
            return pot;
        }
    }
}

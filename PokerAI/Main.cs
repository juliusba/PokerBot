using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

namespace PokerAI
{
	class MainClass
	{
        private static readonly int preFlopRollOutCount = 10000;

        public static void Main (string[] args)
		{
            MainClass m = new MainClass();
            Stopwatch watch = new Stopwatch();
            watch.Start();
            Table table = new Table(6);
            table.Play();
            watch.Stop();
            Console.WriteLine(
                string.Format("Minutes :{0}\nSeconds :{1}\n Mili seconds :{2}",
                watch.Elapsed.Minutes, watch.Elapsed.Seconds, watch.Elapsed.TotalMilliseconds));
            int i = 0;
		}

        public static void PreFlopRollOut()
        {
            List<Card> holeCards = new List<Card>();
            List<Card> otherHand = new List<Card>();
            List<Card> communityCards = new List<Card>();
            List<Card> myHand = new List<Card>();
            PowerRating myPowerRating;
            PowerRating otherPowerRating;
            int numWins;
            bool beaten;
            Deck deck = new Deck();
            for (int p = 2; p <= 10; p++)
            {
                string file = "";
                for (int c1 = 2; c1 < 15; c1++)
                {
                    for (int c2 = 2; c2 < 15; c2++)
                    {
                        for (int s = 0; s < 2; s++)
                        {
                            holeCards.Clear();
                            holeCards.Add(new Card(Suit.CLOVES, c1));
                            if (s == 0) holeCards.Add(new Card(Suit.SPADES, c2));
                            else
                            {
                                if (c1 == c2) break;
                                holeCards.Add(new Card(Suit.CLOVES, c2));
                            }
                            numWins = 0;
                            for (int x = 0; x < preFlopRollOutCount; x++)
                            {
                                deck.Shuffle(holeCards);
                                communityCards.Clear();
                                communityCards.Add(deck.DrawCard());
                                communityCards.Add(deck.DrawCard());
                                communityCards.Add(deck.DrawCard());
                                communityCards.Add(deck.DrawCard());
                                communityCards.Add(deck.DrawCard());
                                myHand.Clear();
                                myHand.AddRange(holeCards);
                                myHand.AddRange(communityCards);
                                myPowerRating = new PowerRating(myHand);
                                beaten = false;
                                int k = 1;
                                while (!beaten && k < p)
                                {
                                    otherHand.Clear();
                                    otherHand.Add(deck.DrawCard());
                                    otherHand.Add(deck.DrawCard());
                                    otherHand.AddRange(communityCards);
                                    otherPowerRating = new PowerRating(otherHand);
                                    beaten = !(myPowerRating.betterThan(otherPowerRating) == 1);
                                    k++;
                                }
                                if (!beaten) numWins++;
                            }
                            file += numWins;
                            if (s == 0)
                                file += "-";
                            else
                                file += "|";
                        }
                    }
                    file += System.Environment.NewLine;
                }
                System.IO.File.WriteAllText(@"E:\Users\Julius\Desktop\preflop" + p + ".txt", file);
            }
        }

        public static double getWins(List<Card> hand, int playerCount)
        {
            string fileName = @"E:\Users\Julius\Desktop\preflop" + playerCount + ".txt";
            int val1 = hand[0].Value;
            int val2 = hand[1].Value;
            int suited = hand[0].Suit == hand[1].Suit ? 1 : 0;
            try
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    for (int i = 1; i < val1; i++)
                        sr.ReadLine();
                    return (int.Parse(sr.ReadLine().Split('|')[val2].Split('-')[suited])) / preFlopRollOutCount;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            return 0;
        }

        //public static String preflop2 = "";
        //public static String preflop3 = "";
        //public static String preflop4 = "";
        //public String preflop5 = "";
        //public String preflop6 = "";
        //public String preflop7 = "";
        //public String preflop8 = "";
        //public String preflop9 = "";
        //public String preflop10 = "";

        //public static void addStats(int c1, int c2, int s, int p, double winPercentage)
        //{
        //    String end;
        //    if (s == 0)
        //        end = ",";
        //    else
        //        end = "|";

        //    if (c2 == 12 && s == 1)
        //    {
        //        end = System.Environment.NewLine;
        //    }
        //    switch (p)
        //    {
        //        case 0:
        //            preflop2 += winPercentage + end;
        //            break;
        //        case 1:
        //            preflop3 += winPercentage + end;
        //            break;
        //        case 2:
        //            preflop4 += winPercentage + end;
        //            break;
        //        case 3:
        //            preflop5 += winPercentage + end;
        //            break;
        //        case 4:
        //            preflop6 += winPercentage + end;
        //            break;
        //        case 5:
        //            preflop7 += winPercentage + end;
        //            break;
        //        case 6:
        //            preflop8 += winPercentage + end;
        //            break;
        //        case 7:
        //            preflop9 += winPercentage + end;
        //            break;
        //        case 8:
        //            preflop10 += winPercentage + end;
        //            break;
        //    }
        //}

        //public static string readFromFile(string fileName)
        //{
        //    string fromFile = "";
        //    try
        //    {
        //        using (StreamReader sr = new StreamReader(fileName))
        //        {
        //            string line = sr.ReadToEnd();
        //            Console.WriteLine(line);
        //            return fromFile;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("The file could not be read:");
        //        Console.WriteLine(e.Message);
        //    }
        //    return fromFile;
        //}

        /*
        public List<PreflopHandVal> fileToObject(string file)
        {
            List<PreflopHandVal> hands = new List<PreflopHandVal>();

            string[] handOn = file.Split('|');

            for (int i = 0; i < hand.GetLength; i++ )

                return hands;
        }

        public class PreflopHandVal
        {

            public int c1 { get; set; }
            public int c2 { get; set; }
            public bool s { get; set; }
            public int p { get; set; }
            public double val { get; set; }

            public PreflopHandVal(int c1, int c2, bool s, int p, double val)
            {
                this.c1 = c1;
                this.c2 = c2;
                this.s = s;
                this.p = p;
                this.val = val;
            }
            public PreflopHandVal() { }

            public bool exists(int c1, int c2, bool s, int p)
            {
                return this.c1 == c1 && this.c2 == c2 && this.s == s && this.p == p;
            }
        }
        */
	}
}

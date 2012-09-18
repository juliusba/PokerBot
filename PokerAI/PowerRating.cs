using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerAI
{
    class PowerRating
    {
        public int first { get; private set; }
        public int second { get; private set; }
        public int third { get; private set; }
        public int fourth { get; private set; }
        public int fift { get; private set; }
        public int sixth { get; private set; }


        public PowerRating(List<Card> hand)
        {
            string s = "";
            foreach (Card c in hand) s += c.ToString() + " ";
            //Console.WriteLine(s);
            if (hand.Count == 2)
            {
                if (hand[0].Value == hand[1].Value)
                {
                    first = (hand[0].Value / 3) * 2;
                }
                else if (hand[0].Suit == hand[1].Suit)
                {
                    first = 6;
                }
                else
                {
                    first = (hand[0].Value / 2);
                }
                return;
            }
            hand = hand.OrderByDescending(c => c.Value).ToList();
            if (flushORstraightFlush(hand)) return;

            var ValueGroup = hand.GroupBy(c => c.Value).Select(g => new { value = g.Key, count = g.Count(), cards = g }).ToList();
            var valueGroupByCount = ValueGroup.OrderByDescending(vc => vc.count).ToList();
            int highestQuantity = valueGroupByCount.First().count;
            int highestQuantityValue = valueGroupByCount.First().value;

            if (highestQuantity == 4)
            {
                #region 4 of a kind
                first = 7;
                second = highestQuantityValue;
                third = ValueGroup[0].value == second ? ValueGroup[1].value : ValueGroup[0].value;
                #endregion
            }
            else
            {
                if (highestQuantity == 3 && valueGroupByCount[1].count > 1)
                {
                    #region Full house
                    first = 6;
                    second = highestQuantityValue;

                    if (valueGroupByCount.Count > 2 && valueGroupByCount[2].count > 1)
                        third = Math.Max(valueGroupByCount[1].value, valueGroupByCount[2].value);
                    else
                        third = valueGroupByCount[1].value;

                    #endregion
                }
                else
                {
                    #region Check for straight
                    int i = 0;
                    while (i + 4 < ValueGroup.Count)
                    {
                        if (ValueGroup[i].value == ValueGroup[i + 4].value + 4)
                        {
                            first = 4;
                            second = ValueGroup[i].value;
                            return;
                        }
                        i++;
                    }
                    #endregion

                    if (highestQuantity == 3)
                    {
                        #region 3 of a kind
                        first = 3;
                        second = highestQuantityValue;

                        if (ValueGroup[0].value == second)
                            third = ValueGroup[1].value;
                        else
                            third = ValueGroup[0].value;

                        if (ValueGroup[1].value == second || ValueGroup[1].value == third)
                            fourth = ValueGroup[2].value;
                        else
                            fourth = ValueGroup[1].value;
                        #endregion
                    }
                    else if (highestQuantity == 2)
                    {
                        if (valueGroupByCount[1].count > 1)
                        {
                            #region Two pairs

                            first = 2;
                            second = highestQuantityValue;

                            if (valueGroupByCount[2].count > 1)
                                third = Math.Max(valueGroupByCount[1].value, valueGroupByCount[2].value);
                            else
                                third = valueGroupByCount[1].value;

                            fourth = ValueGroup.First(vc => vc.value != second && vc.value != third).value;

                            #endregion
                        }
                        else
                        {
                            #region One Pair
                            first = 1;
                            second = highestQuantityValue;
                            i = 0;
                            if (ValueGroup[0].value == highestQuantityValue) i = 1;
                            third = ValueGroup[i].value;
                            if (ValueGroup[1].value == highestQuantityValue) i = 1;
                            fourth = ValueGroup[i + 1].value;
                            if (ValueGroup[2].value == highestQuantityValue) i = 1;
                            fift = ValueGroup[i + 2].value;

                            return;
                            #endregion
                        }
                    }
                    else
                    {
                        #region Highcard
                        first = 0;
                        second = highestQuantityValue;
                        third = ValueGroup[1].value;
                        fourth = ValueGroup[2].value;
                        fift = ValueGroup[3].value;
                        sixth = ValueGroup[4].value;
                        #endregion
                    }
                }
            }
        }

        #region Unused code
        //var ValueGroup = hand.GroupBy(c => c.Value).Select(g => new { value = g.Key, count = g.Count() }).OrderBy(vc => vc.count).ThenBy(vc => vc.value).ToList();
        //switch (ValueGroup[0].count)
        //{
        //    case 4:
        //        break;
        //}


        //List<int> valueCount = new List<int>(13){0,0,0,0,0,0,0,0,0,0,0,0,0};
        //List<int> suitCount = new List<int>(4){0,0,0,0};

        //foreach (Card c in hand)
        //{
        //    suitCount[(int)c.Suit]++;
        //    valueCount[c.Value]++;
        //}
        //bool flush = suitCount.Max() >= 5;
        //int straightCount = 0;
        //int highest = -1;
        //bool straight = false;
        //for (int i = 12; i >= 0; i--)
        //{
        //    if (valueCount[i] > 0)
        //    {
        //        straightCount++;
        //        if (straightCount == 5)
        //        {
        //            straight = true;
        //            highest = i+4;
        //            break;
        //        }
        //    }
        //    else straightCount = 0;
        //}
        //if (straightCount >= 4 && valueCount[12] > 0)
        //{
        //    straight = true;
        //    highest = 12;
        //}

        //if (flush & straight)
        //{
        //    List<Card> temp = hand.Where(c => ((int) c.Suit) == suitCount.IndexOf(suitCount.Max())).OrderByDescending(c => c.Value).ToList();
        //    int i = 0;
        //    while (i + 4 < temp.Count)
        //    {
        //        if (temp[i].Value == temp[i + 4].Value + 4)
        //        {
        //            first = 8;
        //            second = temp[i].Value;
        //            return;
        //        }
        //        i++;
        //    }
        //}

        //int highestValueQuantity = ValueGroup.Max();

        //switch (highestValueQuantity)
        //{
        //    case 4:
        //        first = 7;
        //        second = ValueGroup.IndexOf(4);
        //        third = Math.Max(Math.Max(ValueGroup.IndexOf(3), ValueGroup.IndexOf(2)), ValueGroup.IndexOf(1));
        //        break;
        //    case 3:
        //        first = 6;
        //        second = ValueGroup.LastIndexOf(3);
        //        third = Math.Max(ValueGroup.LastIndexOf(second - 1, 3), ValueGroup.LastIndexOf(2));
        //        if (third == -1)
        //        {
        //            first = 3;
        //            third = ValueGroup.LastIndexOf(1);
        //            fourth = ValueGroup.FindLastIndex(third - 1, v => v == 1);
        //        }
        //        break;
        //    case 2:
        //        if (!straight && !flush)
        //        {
        //            second = ValueGroup.LastIndexOf(2);
        //            third = ValueGroup.FindLastIndex(second - 1, v => v == 2);
        //            if (third != -1)
        //            {
        //                first = 2;
        //                fourth = ValueGroup.LastIndexOf(1);
        //            }
        //            else
        //            {
        //                first = 1;
        //                third = ValueGroup.LastIndexOf(1);
        //                fourth = ValueGroup.FindLastIndex(third - 1, v => v == 1);
        //                fift = ValueGroup.FindLastIndex(fourth - 1, v => v == 1);
        //            }
        //        }
        //        break;
        //    case 1:
        //        if (!straight && !flush)
        //        {
        //            first = 0;
        //            second = ValueGroup.LastIndexOf(1);
        //            third = ValueGroup.FindLastIndex(second - 1, v => v == 0);
        //            fourth = ValueGroup.FindLastIndex(third - 1, v => v == 0);
        //            fift = ValueGroup.FindLastIndex(fourth - 1, v => v == 0);
        //            sixth = ValueGroup.FindLastIndex(fift - 1, v => v == 0);
        //        }
        //        break;
        //}
        //}

        //private straight(var )
        //{
        //    while (i + 4 < ValueGroup.Count)
        //        {
        //            if (ValueGroup[i].value == ValueGroup[i + 4].value + 4)
        //            {
        //                first = 4;
        //                second = ValueGroup[i].Value;
        //                return;
        //            }
        //            i++;
        //        }
        //}
        #endregion

        private bool flushORstraightFlush(List<Card> hand)
        {
            var flushGroup = hand.GroupBy(c => c.Suit).FirstOrDefault(g => g.Count() >= 5);
            bool flush = flushGroup != null;
            if (flush)
            {
                var flushCards = flushGroup.ToList();
                int i = 0;
                while (i + 4 < flushCards.Count)
                {
                    if (flushCards[i].Value == flushCards[i + 4].Value + 4)
                    {
                        first = 8;
                        second = flushCards[i].Value;
                        return true;
                    }
                    i++;
                }
                first = 5;
                second = flushCards[0].Value;
                third = flushCards[1].Value;
                fourth = flushCards[2].Value;
                fift = flushCards[3].Value;
                sixth = flushCards[4].Value;
                return true;
            }
            return false;
        }

        public int betterThan(PowerRating other)
        {
            if (this.first != other.first) return this.first > other.first ? 1 : -1;
            else if (this.second != other.second) return this.second > other.second ? 1 : -1;
            else if (this.third != other.third) return this.third > other.third ? 1 : -1;
            else if (this.fourth != other.fourth) return this.fourth > other.fourth ? 1 : -1;
            else if (this.fift != other.fift) return this.fift > other.fift ? 1 : -1;
            else if (this.sixth != other.sixth) return this.sixth > other.sixth ? 1 : -1;
            else return 0;
        }

        public string ToString()
        {
            return "(" + first + ", " + second + ", " + third + ", " + fourth + ", " + fift + ", " + sixth + ")";
        }

        public bool betterThan(List<Card> hand)
        {
            /*
            hand = hand.OrderByDescending(c => c.Value).ToList();
            var ValueGroup = hand.GroupBy(c => c.Value).Select(g => new { value = g.Key, count = g.Count(), cards = g }).ToList();
            var valueGroupByCount = ValueGroup.OrderByDescending(vc => vc.count).ToList();
            int highestQuantity = valueGroupByCount.First().count;
            int highestQuantityValue = valueGroupByCount.First().value;

            bool falseIfStraight;
            bool falseIfFlush;
            bool falseIfStraightFlush;

            bool temp = false;
            switch (first)
            {
                case 0:
                    if (valueGroupByCount[0].value > 1 || straightOrFlush(hand))
                        return true;

                    else if (second > ValueGroup[0].value) return true;
                    else if (third > ValueGroup[1].value) return true;
                    else if (fourth > ValueGroup[2].value) return true;
                    else if (fift > ValueGroup[3].value) return true;
                    else
                    {
                        temp = sixth >= ValueGroup[4].value;

                    }
                    
                    break;
                case 1:
                    if (valueGroupByCount[0].value > 2 || (valueGroupByCount[0].value == 2 && valueGroupByCount[1].count > 2))
                        return true;
                    else if (valueGroupByCount[0].value == 1)
                        return false;
                    else
                    {
                        if (straightOrFlush(hand)) return false;
                    }
            
                    return 0;

                    break;
            }
            */

            var ValueGroup = hand.GroupBy(c => c.Value).Select(g => new { value = g.Key, count = g.Count(), cards = g }).ToList();
            var valueGroupByCount = ValueGroup.OrderByDescending(vc => vc.count).ToList();
            int highestQuantity = valueGroupByCount.First().count;
            int highestQuantityValue = valueGroupByCount.First().value;

            var flushGroup = hand.GroupBy(c => c.Suit).FirstOrDefault(g => g.Count() >= 5);
            bool flush = flushGroup != null;
            if (flush)
            {
                if (first < 5)
                    return true;

                var flushCards = flushGroup.ToList();
                int i = 0;
                while (i + 4 < flushCards.Count)
                {
                    if (flushCards[i].Value == flushCards[i + 4].Value + 4)
                    {
                        return first == 8 && second >= flushCards[i].Value;
                    }
                    i++;
                }
                if (first == 5)
                    return second >= flushCards[0].Value;
            }
            if (first == 8) return true;

            if (highestQuantity == 4)
            {
                if (first == 7)
                {
                    if (second > highestQuantityValue)
                        return true;
                    else if (second == highestQuantityValue)
                    {
                        int otherThird = ValueGroup[0].value == second ? ValueGroup[1].value : ValueGroup[0].value;
                        return third >= otherThird;
                    }
                    return false;
                }
                else
                {
                    if (highestQuantity == 3 && valueGroupByCount[1].count > 1)
                    {
                        if (first == 6)
                        {
                            if (second > highestQuantityValue)
                                return true;
                            else if (second == highestQuantityValue)
                            {
                                if (valueGroupByCount.Count > 2 && valueGroupByCount[2].count > 1)
                                    return third >= Math.Max(valueGroupByCount[1].value, valueGroupByCount[2].value);
                                else
                                    return third >= valueGroupByCount[1].value;
                            }
                        }
                        return false;
                    }
                    else
                    {
                        if (first > 4) return true;
                        int i = 0;
                        while (i + 4 < ValueGroup.Count)
                        {
                            if (ValueGroup[i].value == ValueGroup[i + 4].value + 4)
                            {
                                if (first == 4 && second >= ValueGroup[i].value) return true;
                                return false;
                            }
                            i++;
                        }
                        if (first == 4) return true;
                        if (highestQuantity == 3)
                        {
                            if (first == 3)
                            {
                                int tempSecond = highestQuantityValue;
                                if (second > tempSecond) return true;

                                if (second == tempSecond)
                                {
                                    int tempThird = 0;
                                    if (ValueGroup[0].value == second)
                                        tempThird = ValueGroup[1].value;
                                    else
                                        tempThird = ValueGroup[0].value;
                                    if (third > tempThird) return true;

                                    if (third == tempThird)
                                    {
                                        if (ValueGroup[1].value == second || ValueGroup[1].value == third)
                                            return fourth >= ValueGroup[2].value;
                                        else
                                            return fourth >= ValueGroup[1].value;
                                    }
                                    
                                }
                            }
                            return false;
                        }
                        if(first == 3) return true;
                        if (highestQuantity == 2)
                        {
                            if (valueGroupByCount[1].count > 1)
                            {
                                if (first == 2)
                                {
                                    if (second > highestQuantityValue) return true;

                                    if (second == highestQuantityValue)
                                    {
                                        int tempThird = 0;
                                        if (valueGroupByCount[2].count > 1)
                                            tempThird = Math.Max(valueGroupByCount[1].value, valueGroupByCount[2].value);
                                        else
                                            tempThird = valueGroupByCount[1].value;
                                        
                                        if (third > tempThird) return true;
                                        else if (third == tempThird && fourth >= ValueGroup.First(vc => vc.value != second && vc.value != third).value) return true;
                                    }
                                }
                                return false;
                            }
                            if (first == 2) return true;
                            if(first == 1)
                            {
                                if (second > highestQuantityValue) return true;
                                if (second == highestQuantityValue)
                                {
                                    if (third > ValueGroup[1].value) return true;
                                    if (third == ValueGroup[1].value)
                                    {
                                        if (fourth > ValueGroup[2].value) return true;
                                        return fourth == ValueGroup[2].value && fift >= ValueGroup[3].value;
                                    }
                                }
                            }
                            return false;
                        }
                        else
                        {
                            if (second > highestQuantityValue) return true;
                            if (second == highestQuantityValue)
                            {
                                if (third > ValueGroup[1].value) return true;
                                if (third == ValueGroup[1].value)
                                {
                                    if (fourth > ValueGroup[2].value) return true;
                                    if(fourth == ValueGroup[2].value)
                                    {
                                        if (fift > ValueGroup[3].value) return true;
                                        return fift == ValueGroup[3].value && sixth >= ValueGroup[4].value;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
        /*
        private static int betterThanZero(List<Card> hand)
        {
            hand = hand.OrderByDescending(c => c.Value).ToList();
            var ValueGroup = hand.GroupBy(c => c.Value).Select(g => new { value = g.Key, count = g.Count(), cards = g }).ToList();
            if (ValueGroup.OrderByDescending(vc => vc.count).First().value > 1 || straightOrFlush(hand))
                return 1;
            
            return 0;
        }

        private static int betterThanOne(List<Card> hand)
        {
            hand = hand.OrderByDescending(c => c.Value).ToList();
            var ValueGroup = hand.GroupBy(c => c.Value).Select(g => new { value = g.Key, count = g.Count(), cards = g }).ToList();
            var valueGroupByCount = ValueGroup.OrderByDescending(vc => vc.count).ToList();
            if (valueGroupByCount[0].value > 2 || (valueGroupByCount[0].value == 2 && valueGroupByCount[1].count > 2) || straightOrFlush(hand))
                return 1;
            else if (valueGroupByCount[0].value == 1)
                return -1;
            
            return 0;
        }

        private static int betterThanTwo(List<Card> hand)
        {
            hand = hand.OrderByDescending(c => c.Value).ToList();
            var ValueGroup = hand.GroupBy(c => c.Value).Select(g => new { value = g.Key, count = g.Count(), cards = g }).ToList();
            var valueGroupByCount = ValueGroup.OrderByDescending(vc => vc.count).ToList();
            if (valueGroupByCount[0].value > 2 || straightOrFlush(hand))
                return 1;
            else if (valueGroupByCount[0].value < 2 || valueGroupByCount[1].count == 1)
                return -1;
            
            return 0;
        }
        private static int betterThanThree(List<Card> hand)
        {
            hand = hand.OrderByDescending(c => c.Value).ToList();
            var ValueGroup = hand.GroupBy(c => c.Value).Select(g => new { value = g.Key, count = g.Count(), cards = g }).ToList();
            var valueGroupByCount = ValueGroup.OrderByDescending(vc => vc.count).ToList();
            if (valueGroupByCount[0].value > 3 || (valueGroupByCount[0].value == 3 && valueGroupByCount[1].count > 1) || flushOrStraightFlush(hand) || straight(hand))
                return 1;
            else if (valueGroupByCount[0].value < 3)
                return -1;
            
            return 0;
        }

        private static int betterThanFour(List<Card> hand)
        {
            hand = hand.OrderByDescending(c => c.Value).ToList();
            if(flushOrStraightFlush(hand))
                return 1;
            
            var ValueGroup = hand.GroupBy(c => c.Value).Select(g => new { value = g.Key, count = g.Count(), cards = g }).ToList();
            var valueGroupByCount = ValueGroup.OrderByDescending(vc => vc.count).ToList();
            if (valueGroupByCount[0].value > 3 || (valueGroupByCount[0].value == 3 && valueGroupByCount[1].count > 1))
                return 1;
            else if (!straight(ValueGroup))
                return -1;
            
            return 0;
        }

        private static int betterThanFive(List<Card> hand)
        {
            hand = hand.OrderByDescending(c => c.Value).ToList();
            var ValueGroup = hand.GroupBy(c => c.Value).Select(g => new { value = g.Key, count = g.Count(), cards = g }).ToList();
            var valueGroupByCount = ValueGroup.OrderByDescending(vc => vc.count).ToList();
            if (valueGroupByCount[0].value > 3 || (valueGroupByCount[0].value == 3 && valueGroupByCount[1].count > 1) || straightFlush(hand))
                return 1;
            else if (!flush(hand))
                return -1;

            return 0;
        }

        private static int betterThanSix(List<Card> hand)
        {
            hand = hand.OrderByDescending(c => c.Value).ToList();
            var ValueGroup = hand.GroupBy(c => c.Value).Select(g => new { value = g.Key, count = g.Count(), cards = g }).ToList();
            var valueGroupByCount = ValueGroup.OrderByDescending(vc => vc.count).ToList();
            if (valueGroupByCount[0].value > 3 || straightFlush(hand))
                return 1;
            else if (!(valueGroupByCount[0].value == 3 && valueGroupByCount[1].count > 1))
                return -1;

            return 0;
        }

        private static int betterThanSeven(List<Card> hand)
        {
            hand = hand.OrderByDescending(c => c.Value).ToList();
            if (straightFlush(hand))
                return 1;
            else
            {
                var ValueGroup = hand.GroupBy(c => c.Value).Select(g => new { value = g.Key, count = g.Count(), cards = g }).ToList();
                var valueGroupByCount = ValueGroup.OrderByDescending(vc => vc.count).ToList();
                if (!(valueGroupByCount[0].value < 4))
                    return -1;
            }

            return 0;
        }

        private static bool straight(List<Card> hand)
        {
            int i = 0;
            while (i + 4 < ValueGroup.Count)
            {
                if (ValueGroup[i].value == ValueGroup[i + 4].value + 4)
                    return true;
                i++;
            }
            return false;
        }

        private static bool flush(List<Card> hand)
        {
            return false;
        }

        private static bool straightOrFlush(List<Card> hand)
        {
            return false;
        }

        private static bool flushOrStraightFlush(List<Card> hand)
        {
            return false;
        }

        private static bool straightFlush(List<Card> hand)
        {
            return false;
        }
        */
    }
}
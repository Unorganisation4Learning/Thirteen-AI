using Game.Casino;
using System.Collections.Generic;
using System.Linq;

namespace Game.Thirteen
{
    public class ThirteenContinuouslyPairsCardCombination : ThirteenPairCombination
    {
        public struct Filter
        {
            public int StartByValue;
            public int NumberOfPairs;
        }

        protected StraightCardCombination StraightCombination;

        public ThirteenContinuouslyPairsCardCombination(List<ICard> cards)
            : base(cards, ThirteenCardCombinationComparer.CombinationNameContinuoslyPairs)
        {
            StraightCombination = new ThirteenStraightCombination(cards);
            Pairs = GroupCardByValue(cards);
            List<ECardName> removeKeys = new List<ECardName>();
            foreach (KeyValuePair<ECardName, List<ICard>> it in Pairs)
            {
                // This will contains the cards that have at least 2 of a kind
                if (it.Value.Count < 2)
                {
                    removeKeys.Add(it.Key);
                }
            }
            // remove card name that are not pair
            for (int i = 0; i < removeKeys.Count; i++)
            {
                Pairs.Remove(removeKeys[i]);
            }
        }

        public List<ICard> GetContinouslyPairValues(Filter filter)
        {
            Dictionary<ECardName, List<ICard>> clonedPairs = new Dictionary<ECardName, List<ICard>>(Pairs);

            // The 2 cannot be inside the Straight and Continuously Pairs
            clonedPairs.Remove(ECardName.Two);

            if (Pairs.Count < filter.NumberOfPairs)
            {
                return null;
            }

            // All cards in the dictionary are pair, so just use value of each pair to find a Straight
            List<ICard> distinctCards = new List<ICard>();
            foreach (KeyValuePair<ECardName, List<ICard>> it in clonedPairs)
            {
                distinctCards.Add(it.Value[0]);
            }

            // Lookup a Straight that make by the Pair values
            StraightCombination.SetInitalCards(distinctCards);
            List<ICard> cardsInStraight = StraightCombination.Lookup(new StraightCardCombination.Filter()
            {
                NumberOfCard = filter.NumberOfPairs,
                StartByValue = filter.StartByValue,
                SuitsValidForTheLastCard = EFrenchSuit.Club | EFrenchSuit.Spade | EFrenchSuit.Diamond | EFrenchSuit.Heart
            });

            // If the Straight is found, you have a continuously pairs
            if (cardsInStraight != null && filter.NumberOfPairs == cardsInStraight.Count)
            {
                // Make result list, that contains the Continuously Pair
                List<ICard> outResult = new List<ICard>();
                for (int i = 0; i < cardsInStraight.Count; i++)
                {
                    ICard card = cardsInStraight[i];
                    List<ICard> sameKindOfValueCards = clonedPairs[card.CardName];
                    outResult.AddRange(sameKindOfValueCards.GetRange(0, 2));
                }
                return outResult;
            }

            return null;
        }

        public List<ICard> FirstContinouslyPairValues(int lenghtOfStraight)
        {
            // These are possible a start value
            ECardName[] startValues = new ECardName[]
            {
                ECardName.Three,
                ECardName.Four,
                ECardName.Five,
                ECardName.Six,
                ECardName.Seven,
                ECardName.Eight,
                ECardName.Nine,
                ECardName.Ten,
                ECardName.J,
                ECardName.Q,
            };

            for (int i = 0; i < startValues.Length; i++)
            {
                CasinoCard startCard = new CasinoCard(EFrenchSuit.Spade, startValues[i]);

                List<ICard> result = GetContinouslyPairValues(new Filter()
                {
                    NumberOfPairs = lenghtOfStraight,
                    StartByValue = startCard.GetCardValue(),
                });

                if (result != null && result.Count > 0)
                {
                    return result;
                }
            }
            return null;
        }

        public override bool IsValid()
        {
            return ThirteenContinuouslyPairsCardCombination.IsValid(CombinationCards, Pairs);
        }

        public static bool IsValid(List<ICard> checkingCards, Dictionary<ECardName, List<ICard>> pairs)
        {
            if (checkingCards == null || checkingCards.Count != pairs.Count * 2 || pairs.Count <= 0)
            {
                return false;
            }

            List<ICard> newCheckingCards = (pairs.Where(pair => pair.Value.Count > 0).Select(pair => pair.Value[0])).ToList();
            newCheckingCards.Sort();

            int preValue = newCheckingCards[0].GetCardValue();
            for (int i = 1; i < newCheckingCards.Count; i++)
            {
                int curValue = newCheckingCards[i].GetCardValue();
                if (preValue + 1 != curValue)
                {
                    return false;
                }
                preValue = curValue;
            }

            return true;
        }

        public override int CompareTo(ICardCombination other)
        {
            if (other is ThirteenFourOfKindCardCombination)
            {
                List<ICard> fourContinouslyPairCards = this.FirstContinouslyPairValues(4);
                if (fourContinouslyPairCards != null && fourContinouslyPairCards.Count > 0)
                {
                    // four continously pair always geater four of kind.
                    return 1;
                }

                return -1;
            }

            if (other is ThirteenContinuouslyPairsCardCombination otherCom)
            {
                int valueCompare = CombinationCards.Count.CompareTo(otherCom.CombinationCards.Count);
                if (valueCompare != 0)
                {
                    return valueCompare;
                }
                // Compare the largest card, because all value is same
                return GetHighestCard(CombinationCards).CompareTo(GetHighestCard(otherCom.CombinationCards));
            }

            if (other is ThirteenPairCombination pairCom)
            {
                if (pairCom.Pairs.ContainsKey(ECardName.Two))
                {
                    List<ICard> fourContinuouslyPairCards = this.FirstContinouslyPairValues(4);
                    if (fourContinuouslyPairCards != null && fourContinuouslyPairCards.Count > 0)
                    {
                        // four continuously pair always getter pair two.
                        return 1;
                    }
                }
            }

            if (other is ThirteenHighCardCombination highCom)
            {
                if (highCom.OwnerCards[0].CardName == ECardName.Two)
                {
                    // anny continously pair always geater single two.
                    return 1;
                }
            }

            return -999;
        }
    }

    /// <summary>
    /// Class is used to check if 2 ContinuouslyPairsCardCombination can compare to each other
    /// </summary>
    public class ContinuoslyPairsCardComparableValidator : ICardCombinationComparableValidator
    {
        public virtual bool IsComparable(ICardCombination a, ICardCombination b)
        {
            if (a == null || a is not ThirteenContinuouslyPairsCardCombination || b == null || b is not ThirteenContinuouslyPairsCardCombination)
            {
                return false;
            }

            if (a.OwnerCards == null || b.OwnerCards == null)
            {
                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// The comparer is used to compare 2 ContinuoslyPairsCardComparable
    /// </summary>
    public class ContinuoslyPairsCardComparer : CardCombinationComparer
    {
        public ContinuoslyPairsCardComparer()
        {
            ComparableValidator = new ContinuoslyPairsCardComparableValidator();
        }
    }

    /// <summary>
    /// Class is used to check if ContinuoslyPairsCard and FourOfKindCard can compare to each other
    /// </summary>
    public class ContinuoslyPairsCard_FourOfKindCardComparableValidator : ICardCombinationComparableValidator
    {
        public virtual bool IsComparable(ICardCombination a, ICardCombination b)
        {
            if (a == null || a is not ThirteenContinuouslyPairsCardCombination || b == null || b is not ThirteenFourOfKindCardCombination)
            {
                return false;
            }

            if (a.OwnerCards == null || b.OwnerCards == null)
            {
                return false;
            }

            foreach (ICard card in b.OwnerCards)
            {
                if (card.CardName == ECardName.Two)
                {
                    return false;
                }
            }

            return true;
        }
    }

    /// <summary>
    /// The comparer is used to compare ContinuoslyPairsCard and FourOfKindCard
    /// </summary>
    public class ContinuoslyPairsCard_FourOfKindCardComparer : CardCombinationComparer
    {
        public ContinuoslyPairsCard_FourOfKindCardComparer()
        {
            ComparableValidator = new ContinuoslyPairsCard_FourOfKindCardComparableValidator();
        }
    }

    /// <summary>
    /// Class is used to check if ContinuoslyPairsCard and Single Two can compare to each other
    /// </summary>
    public class ContinuoslyPairsCard_SingleTwoCardComparableValidator : ICardCombinationComparableValidator
    {
        public virtual bool IsComparable(ICardCombination a, ICardCombination b)
        {
            if (a == null || a is not ThirteenContinuouslyPairsCardCombination || b == null || b is not ThirteenHighCardCombination)
            {
                return false;
            }

            if (a.OwnerCards == null || b.OwnerCards == null)
            {
                return false;
            }

            if (b.OwnerCards[0].CardName != ECardName.Two)
            {
                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// The comparer is used to compare  ContinuoslyPairsCard and Single Two
    /// </summary>
    public class ContinuoslyPairsCard_SingleTwoCardComparer : CardCombinationComparer
    {
        public ContinuoslyPairsCard_SingleTwoCardComparer()
        {
            ComparableValidator = new ContinuoslyPairsCard_SingleTwoCardComparableValidator();
        }
    }

    /// <summary>
    /// Class is used to check if ContinuoslyPairsCard and Pair Two can compare to each other
    /// </summary>
    public class ContinuoslyPairsCard_PairTwoCardComparableValidator : ICardCombinationComparableValidator
    {
        public virtual bool IsComparable(ICardCombination a, ICardCombination b)
        {
            if (a == null || a is not ThirteenContinuouslyPairsCardCombination || b == null || b is not ThirteenPairCombination)
            {
                return false;
            }

            if (a.OwnerCards == null || b.OwnerCards == null)
            {
                return false;
            }

            foreach (ICard card in b.OwnerCards)
            {
                if (card.CardName != ECardName.Two)
                {
                    return false;
                }
            }

            return true;
        }
    }

    /// <summary>
    /// The comparer is used to compare  ContinuoslyPairsCard and Single Two
    /// </summary>
    public class ContinuoslyPairsCard_PairTwoCardComparer : CardCombinationComparer
    {
        public ContinuoslyPairsCard_PairTwoCardComparer()
        {
            ComparableValidator = new ContinuoslyPairsCard_PairTwoCardComparableValidator();
        }
    }
}

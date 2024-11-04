using Game.Casino;
using System.Collections.Generic;

namespace Game.Thirteen
{
    public class ThirteenPairCombination : NumberOfKindCardCombination
    {
        public Dictionary<ECardName, List<ICard>> Pairs { get; protected set; } = null;

        public ThirteenPairCombination(List<ICard> cards, string combinationName)
            : base(cards, combinationName, 2)
        {
            Init(cards);
        }

        public ThirteenPairCombination(List<ICard> cards)
            : base(cards, ThirteenCardCombinationComparer.CombinationNamePair, 2)
        {
            Init(cards);
        }

        private void Init(List<ICard> cards)
        {
            Pairs = GroupCardByValue(cards);
            List<ECardName> removeKeys = new List<ECardName>();
            foreach (KeyValuePair<ECardName, List<ICard>> it in Pairs)
            {
                if (it.Value.Count != 2)
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

        public List<ICard> GetHighestPair()
        {
            if (Pairs.Count == 0)
            {
                return null;
            }

            ECardName highestKey = ECardName.Unknow;
            ICard highestCard = default;

            foreach (KeyValuePair<ECardName, List<ICard>> it in Pairs)
            {
                if (highestKey == ECardName.Unknow)
                {
                    highestKey = it.Key;
                    highestCard = it.Value[0];
                }
                else
                {
                    if (it.Value[0].CompareTo(highestCard) > 0)
                    {
                        highestKey = it.Key;
                        highestCard = it.Value[0];
                    }
                }
            }
            return new List<ICard>(Pairs[highestKey]);
        }
        public List<ICard> GetSmallestPair()
        {
            if (Pairs.Count == 0)
            {
                return null;
            }

            ECardName smallestKey = ECardName.Unknow;
            ICard smallestCard = default;

            foreach (KeyValuePair<ECardName, List<ICard>> it in Pairs)
            {
                if (smallestKey == ECardName.Unknow)
                {
                    smallestKey = it.Key;
                    smallestCard = it.Value[0];
                }
                else
                {
                    if (it.Value[0].CompareTo(smallestCard) < 0)
                    {
                        smallestKey = it.Key;
                        smallestCard = it.Value[0];
                    }
                }
            }
            return new List<ICard>(Pairs[smallestKey]);
        }

        public override int CompareTo(ICardCombination other)
        {
            if (other is ThirteenContinuouslyPairsCardCombination otherCom)
            {
                List<ICard> fourContinouslyPairCards = otherCom.FirstContinouslyPairValues(4);
                if (fourContinouslyPairCards != null && fourContinouslyPairCards.Count > 0)
                {
                    return -1;
                }
                return -999;
            }

            if (other is ThirteenFourOfKindCardCombination)
            {
                return -1;
            }

            return base.CompareTo(other);
        }
    }

    public class ThirteenOnePairComparableValidator : ICardCombinationComparableValidator
    {
        public bool IsComparable(ICardCombination a, ICardCombination b)
        {
            ThirteenPairCombination aPairs = a as ThirteenPairCombination;
            ThirteenPairCombination bPairs = b as ThirteenPairCombination;

            if (aPairs == null || bPairs == null)
            {
                return false;
            }

            if (!aPairs.IsValid() || !bPairs.IsValid())
            {
                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// Class is used to check if FourOfKindCard vs SingleTwo can compare to each other
    /// </summary>
    public class PairTwoCard_FourOfKindCardComparableValidator : ICardCombinationComparableValidator
    {
        public virtual bool IsComparable(ICardCombination a, ICardCombination b)
        {
            if (a == null || a is not ThirteenPairCombination || b == null || b is not ThirteenFourOfKindCardCombination)
            {
                return false;
            }

            if (a.OwnerCards == null || b.OwnerCards == null)
            {
                return false;
            }

            foreach (ICard card in a.OwnerCards)
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
    /// The comparer is used to compare FourOfKindCard vs PairTwo
    /// </summary>
    public class PairTwoCard_FourOfKindCardComparer : CardCombinationComparer
    {
        public PairTwoCard_FourOfKindCardComparer()
        {
            ComparableValidator = new PairTwoCard_FourOfKindCardComparableValidator();
        }
    }

    /// <summary>
    /// Class is used to check if ContinuoslyPairsCard and Pair Two can compare to each other
    /// </summary>
    public class PairTwoCard_ContinuoslyPairsCardComparableValidator : ICardCombinationComparableValidator
    {
        public virtual bool IsComparable(ICardCombination a, ICardCombination b)
        {
            if (a == null || a is not ThirteenPairCombination || b == null || b is not ThirteenContinuouslyPairsCardCombination)
            {
                return false;
            }

            if (a.OwnerCards == null || b.OwnerCards == null)
            {
                return false;
            }

            foreach (ICard card in a.OwnerCards)
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
    public class PairTwoCard_ContinuoslyPairsCardComparer : CardCombinationComparer
    {
        public PairTwoCard_ContinuoslyPairsCardComparer()
        {
            ComparableValidator = new PairTwoCard_ContinuoslyPairsCardComparableValidator();
        }
    }
}

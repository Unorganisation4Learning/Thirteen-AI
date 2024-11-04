namespace Game.Thirteen
{
    using Game.Core;
    using Game.Casino;
    using System.Collections.Generic;
    using System.Linq;

    public class ThirteenFourOfKindCardCombination : NumberOfKindCardCombination
    {
        public ThirteenFourOfKindCardCombination(List<ICard> cards)
            : base(cards, ThirteenCardCombinationComparer.CombinationNameFourOfKind, 4)
        {

        }

        public override int CompareTo(ICardCombination other)
        {
            if (other is ThirteenContinuouslyPairsCardCombination otherCom)
            {
                List<ICard> fourContinuouslyPairCards = otherCom.FirstContinouslyPairValues(4);
                if (fourContinuouslyPairCards != null && fourContinuouslyPairCards.Count > 0)
                {
                    return -1;
                }

                return 1;
            }

            if (other is ThirteenPairCombination or ThirteenHighCardCombination)
            {
                if (other.OwnerCards.All(card => card.CardName == ECardName.Two))
                {
                    // four of a kind always greater than single and pair two.
                    return 1;
                }
            }

            return base.CompareTo(other);
        }
    }

    /// <summary>
    /// Class is used to check if FourOfKindCard vs Continuously PairsCard can compare to each other
    /// </summary>
    public class FourOfKindCard_ContinuoslyPairsCardComparableValidator : ICardCombinationComparableValidator
    {
        public virtual bool IsComparable(ICardCombination a, ICardCombination b)
        {
            if (a == null || a is not ThirteenFourOfKindCardCombination || b == null || b is not ThirteenContinuouslyPairsCardCombination)
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
    /// The comparer is used to compare FourOfKindCard vs ContinuouslyPairsCard
    /// </summary>
    public class FourOfKindCard_ContinuouslyPairsCardComparer : CardCombinationComparer
    {
        public FourOfKindCard_ContinuouslyPairsCardComparer()
        {
            ComparableValidator = new FourOfKindCard_ContinuoslyPairsCardComparableValidator();
        }
    }

    /// <summary>
    /// Class is used to check if FourOfKindCard vs SingleTwo can compare to each other
    /// </summary>
    public class FourOfKindCard_SingleTwoCardComparableValidator : ICardCombinationComparableValidator
    {
        public virtual bool IsComparable(ICardCombination a, ICardCombination b)
        {
            if (a == null || a is not ThirteenFourOfKindCardCombination || b == null || b is not ThirteenHighCardCombination)
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
    /// The comparer is used to compare FourOfKindCard vs SingleTwo
    /// </summary>
    public class FourOfKindCard_SingleTwoCardComparer : CardCombinationComparer
    {
        public FourOfKindCard_SingleTwoCardComparer()
        {
            ComparableValidator = new FourOfKindCard_SingleTwoCardComparableValidator();
        }
    }

    /// <summary>
    /// Class is used to check if FourOfKindCard vs SingleTwo can compare to each other
    /// </summary>
    public class FourOfKindCard_PairTwoCardComparableValidator : ICardCombinationComparableValidator
    {
        public virtual bool IsComparable(ICardCombination a, ICardCombination b)
        {
            if (a == null || a is not ThirteenFourOfKindCardCombination || b == null || b is not ThirteenPairCombination)
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
    /// The comparer is used to compare FourOfKindCard vs PairTwo
    /// </summary>
    public class FourOfKindCard_PairTwoCardComparer : CardCombinationComparer
    {
        public FourOfKindCard_PairTwoCardComparer()
        {
            ComparableValidator = new FourOfKindCard_PairTwoCardComparableValidator();
        }
    }
}

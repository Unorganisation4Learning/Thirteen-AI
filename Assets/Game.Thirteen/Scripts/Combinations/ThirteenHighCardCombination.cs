namespace Game.Thirteen
{
    using Game.Casino;
    using System.Collections.Generic;

    public class ThirteenHighCardCombination : NumberOfKindCardCombination
    {
        public ThirteenHighCardCombination(List<ICard> cards)
            : base(cards, ThirteenCardCombinationComparer.CombinationNameHighCard, 1)
        {

        }

        public override int CompareTo(ICardCombination other)
        {
            if (other is ThirteenContinuouslyPairsCardCombination otherCom)
            {
                List<ICard> continouslyPairCards = otherCom.FirstContinouslyPairValues(3) ?? otherCom.FirstContinouslyPairValues(4);
                if (continouslyPairCards != null && continouslyPairCards.Count > 0)
                {
                    return -1;
                }
            }

            if (other is ThirteenFourOfKindCardCombination)
            {
                // four of kind always geater single and pair two.
                return -1;
            }

            return base.CompareTo(other);
        }
    }

    /// <summary>
    /// Class is used to check if SingleTwo vs FourOfKindCard can compare to each other
    /// </summary>
    public class SingleTwoCard_FourOfKindCardComparableValidator : ICardCombinationComparableValidator
    {
        public virtual bool IsComparable(ICardCombination a, ICardCombination b)
        {
            if (a == null || a is not ThirteenHighCardCombination || b == null || b is not ThirteenFourOfKindCardCombination)
            {
                return false;
            }

            if (a.OwnerCards == null || b.OwnerCards == null)
            {
                return false;
            }

            if (a.OwnerCards[0].CardName != ECardName.Two)
            {
                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// The comparer is used to compare SingleTwo vs FourOfKindCard
    /// </summary>
    public class SingleTwoCard_FourOfKindCardComparer : CardCombinationComparer
    {
        public SingleTwoCard_FourOfKindCardComparer()
        {
            ComparableValidator = new SingleTwoCard_FourOfKindCardComparableValidator();
        }
    }

    /// <summary>
    /// Class is used to check if Single Two and ContinuoslyPairsCard can compare to each other
    /// </summary>
    public class SingleTwoCard_ContinuoslyPairsCardComparableValidator : ICardCombinationComparableValidator
    {
        public virtual bool IsComparable(ICardCombination a, ICardCombination b)
        {
            if (a == null || a is not ThirteenHighCardCombination || b == null || b is not ThirteenContinuouslyPairsCardCombination)
            {
                return false;
            }

            if (a.OwnerCards == null || b.OwnerCards == null)
            {
                return false;
            }

            if (a.OwnerCards[0].CardName != ECardName.Two)
            {
                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// The comparer is used to compare  ContinuoslyPairsCard and Single Two
    /// </summary>
    public class SingleTwoCard_ContinuoslyPairsCardComparer : CardCombinationComparer
    {
        public SingleTwoCard_ContinuoslyPairsCardComparer()
        {
            ComparableValidator = new SingleTwoCard_ContinuoslyPairsCardComparableValidator();
        }
    }
}

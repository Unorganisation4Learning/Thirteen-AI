namespace Game.Thirteen
{
    using Game.Casino;
    using System.Collections.Generic;
    
    public class ThirteenStraightCombination : StraightCardCombination
    {
        public ThirteenStraightCombination(List<ICard> cards) 
            : base(cards, ThirteenCardCombinationComparer.CombinationNameStraight)
        {

        }

        public override int CompareTo(ICardCombination other)
        {
            if (other is ThirteenStraightCombination otherStraight)
            {
                ICard thisInsCard = CombinationCards[CombinationCards.Count - 1];
                ICard otherInsCard = otherStraight.CombinationCards[otherStraight.CombinationCards.Count - 1];
                return thisInsCard.CompareTo(otherInsCard);
            }
            throw new System.InvalidOperationException("Invalid argument");
        }
    }

    public class ThirteenStraightComparableValidator : StraightComparableValidator
    {
        public override bool IsComparable(ICardCombination a, ICardCombination b)
        {
            // Cannot contain 2 in the Straight
            if (a.OwnerCards.FindIndex(c => c.CardName == ECardName.Two) >= 0 || b.OwnerCards.FindIndex(c => c.CardName == ECardName.Two) >= 0)
            {
                return false;
            }

            return base.IsComparable(a, b);
        }
    }

    public class ThirteenStraightComparer : StraightComparer
    {
        public ThirteenStraightComparer()
        {
            ComparableValidator = new ThirteenStraightComparableValidator();
        }
    }
}

using Game.Casino;
using System.Collections.Generic;

namespace Game.Thirteen
{
    public class ThirteenThreeOfKindCardCombination : NumberOfKindCardCombination
    {
        public ThirteenThreeOfKindCardCombination(List<ICard> cards) 
            : base(cards, ThirteenCardCombinationComparer.CombinationNameThreeOfKind, 3)
        {

        }
    }
}

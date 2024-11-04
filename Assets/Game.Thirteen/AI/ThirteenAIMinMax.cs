using System.Collections;
using System.Collections.Generic;
using Game.Casino;
using Game.Thirteen;

namespace Game.AI
{
    public class ThirteenAIMinMax : IBEAgent
    {
        public ThirteenAIMinMax()
        {

        }

        public CardCombination MakeDecision(CardCombinationManagement mine, CardCombinationManagement other, CardCombination otherComb)
        {
            MMNode root = MinMaxTree.BuildTreeFrom(other, mine, true, otherComb);
            if (root != null && root.Result != null)
            {
                return root.Result.Decision;
            }
            return null;
        }
    }
}

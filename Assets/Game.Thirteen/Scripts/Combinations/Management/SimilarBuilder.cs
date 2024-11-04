namespace Game.Thirteen
{
    using System.Linq;
    using Game.Casino;
    using System.Collections.Generic;

    /// <summary>
    /// Similar builder those pattern same value and same lenght but the suit does not matter
    /// </summary>
    public class SimilarBuilder
    {
        /// <summary>
        /// Will be used to compare with others that can be consider as similar
        /// </summary>
        public readonly CombinationBuilder Inital;

        public List<CombinationBuilder> Similars { get; private set; }

        public SimilarBuilder(CombinationBuilder init)
        {
            Inital = init;
            Similars = new List<CombinationBuilder>() { Inital };
        }

        public bool TryAppend(CombinationBuilder other)
        {
            if (Inital.Pattern.FilledCardId.Count != other.Pattern.FilledCardId.Count)
            {
                return false;
            }

            for (int i = 0; i < other.Pattern.FilledCardId.Count; i++)
            {
                byte thisCardId = Inital.Pattern.FilledCardId[i];
                byte otherCardId = other.Pattern.FilledCardId[i];
                if (CasinoCard.FromCardId(thisCardId).CardName != CasinoCard.FromCardId(otherCardId).CardName)
                {
                    return false;
                }
            }

            Similars.Add(other);
            return true;
        }

        public List<CombinationBuilder> CollectUniqueBuilders()
        {
            // uniqueBuilders those are difference completely( not using any same card)
            List<CombinationBuilder> uniqueBuilders = new List<CombinationBuilder>();
            // Use the cardId as factor of comparision because the higher card is always higher id
            List<CombinationBuilder> clonedSimilars = Similars.OrderByDescending(b => b.Pattern.FilledCardId[b.Pattern.FilledCardId.Count - 1]).ToList();

            while (clonedSimilars.Count > 0)
            {
                CombinationBuilder choosedBuilder = clonedSimilars[0];
                clonedSimilars.RemoveAt(0);
                uniqueBuilders.Add(choosedBuilder);

                // A bit optimize, Early checking for these combination which never can be duplicated
                switch (choosedBuilder.Pattern.PatternName)
                {
                    case ECombination.None:
                    case ECombination.Single:
                    case ECombination.FourOfKind:
                    case ECombination.FourPairs:
                    case ECombination.ThreePairs:
                        {
                            // Skip checking for these types
                            continue;
                        }
                    case ECombination.Pair:
                    case ECombination.Straight:
                    // Many duplicated 3 of kind when you have a 4 of kind
                    case ECombination.ThreeOfKind:
                        {
                            // These types should be checked
                            break;
                        }
                }

                for (int i = clonedSimilars.Count - 1; i >= 0; i--)
                {
                    CombinationBuilder checkingBuilder = clonedSimilars[i];

                    // Checking to see if checkingBuilder is same as choosedBuilder
                    // Remember:: the checkingBuilder.Pattern.FilledCardId card name order is same as choosedBuilder.Pattern.FilledCardId,
                    // if the order is not correct, will happen an unexpect behaviour
                    for (int j = 0; j < checkingBuilder.Pattern.FilledCardId.Count; j++)
                    {
                        byte checkingCardId = checkingBuilder.Pattern.FilledCardId[j];
                        byte choosedCardId = choosedBuilder.Pattern.FilledCardId[j];
                        // Any same card is found, then consider this is a duplicate builder, then it will be removed
                        if (CasinoCard.FromCardId(checkingCardId).Equals(CasinoCard.FromCardId(choosedCardId)))
                        {
                            clonedSimilars.RemoveAt(i);
                            break;
                        }
                    }
                }

            }

            return uniqueBuilders;
        }
    }
}

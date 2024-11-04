namespace Game.Casino
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Class contains card that make a Straigh
    /// </summary>
    public class StraightCardCombination : CardCombination
    {
        public struct Filter
        {
            /// <summary>
            /// How long of the Straight
            /// </summary>
            public int NumberOfCard;
            /// <summary>
            /// The Straight should be started by this value
            /// </summary>
            public int StartByValue;
            /// <summary>
            /// Can be multiple values
            /// </summary>
            public EFrenchSuit SuitsValidForTheLastCard;
        }

        public StraightCardCombination(List<ICard> cards) : base(cards, "Straight")
        {

        }

        public StraightCardCombination(List<ICard> cards, string combinationName) : base(cards, combinationName)
        {

        }

        public List<ICard> Lookup(Filter filter)
        {
            // Invalid Straight, min should be 3
            if (filter.NumberOfCard < 2) return null;

            List<ICard> distinctCards = GetCardsDistinctValue(OwnerCards);
            if (filter.NumberOfCard <= distinctCards.Count)
            {
                List<ICard> outStraight = new List<ICard>();
                int startIdx = distinctCards.FindIndex(c => c.GetCardValue() == filter.StartByValue);
                if (startIdx >= 0)
                {
                    bool isListCountValid = startIdx + filter.NumberOfCard <= distinctCards.Count;

                    // The first of the Straight
                    int preValue = distinctCards[startIdx].GetCardValue();
                    outStraight.Add(distinctCards[startIdx]);

                    // Lookup next value and check if the Straight is continously
                    for (int i = startIdx + 1; i < filter.NumberOfCard; i++)
                    {
                        ICard curCard = distinctCards[i];
                        int curValue = curCard.GetCardValue();
                        if (curValue != preValue + 1)
                        {
                            // This is could not be a Straight
                            return null;
                        }

                        // The value of current card is valid, let add it to the out list
                        preValue = curValue;
                        outStraight.Add(curCard);

                    }

                    EFrenchSuit lastCardSuit = outStraight[outStraight.Count - 1].Suit;
                    // The last card suit match with filter requirement
                    if ((filter.SuitsValidForTheLastCard & lastCardSuit) > 0)
                    {
                        return outStraight;
                    }
                }
            }
            return null;
        }

        public List<List<ICard>> FindAllStraightCard()
        {
            List<List<ICard>> result = new List<List<ICard>>();
            List<ICard> distinctCards = GetCardsDistinctValue(OwnerCards);
            int straightLength = 1;
            List<ICard> straightCards = new List<ICard>();

            for (int i = 0; i < distinctCards.Count - 1; i++)
            {
                ICard curCard = distinctCards[i];
                ICard nextCard = distinctCards[i + 1];

                if (curCard.GetCardValue() == nextCard.GetCardValue() - 1)
                {
                    straightLength++;
                    if (!straightCards.Contains(curCard))
                    {
                        straightCards.Add(curCard);
                    }
                    if (straightLength >= 3)
                    {
                        if (!straightCards.Contains(nextCard))
                        {
                            straightCards.Add(nextCard);
                        }
                        result.Add(straightCards.ToList());
                    }
                }
                else
                {
                    straightCards.Clear();
                    straightLength = 1;
                }
            }

            return result;
        }

        public override bool IsValid()
        {
            return StraightCardCombination.IsValid(CombinationCards);
        }

        public static bool IsValid(List<ICard> checkingCards)
        {
            if (checkingCards == null || checkingCards.Count < 3)
            {
                return false;
            }

            checkingCards.Sort();
            int preValue = checkingCards[0].GetCardValue();
            for (int i = 1; i < checkingCards.Count; i++)
            {
                int curValue = checkingCards[i].GetCardValue();
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
            if (other is StraightCardCombination otherCom)
            {
                return GetHighestCard(CombinationCards).CompareTo(GetHighestCard(otherCom.CombinationCards));
            }
            return 1;
        }
    }


    /// <summary>
    /// Class is used to check if the 2 Straigh are valid and can compare to each other
    /// </summary>
    public class StraightComparableValidator : ICardCombinationComparableValidator
    {
        public virtual bool IsComparable(ICardCombination a, ICardCombination b)
        {
            if (a == null || !(a is StraightCardCombination) || b == null || !(b is StraightCardCombination))
            {
                return false;
            }

            if (!(a is StraightCardCombination straightA) || !straightA.IsValid() || !(b is StraightCardCombination straightB) || !straightB.IsValid())
            {
                return false;
            }

            if (a.OwnerCards == null || b.OwnerCards == null)
            {
                return false;
            }
            if (a.OwnerCards.Count != b.OwnerCards.Count)
            {
                return false;
            }
            return true;
        }
    }


    /// <summary>
    /// Class is used to compare 2 Straight
    /// </summary>
    public class StraightComparer : CardCombinationComparer
    {
        public StraightComparer() : base()
        {
            ComparableValidator = new StraightComparableValidator();
        }
    }
}

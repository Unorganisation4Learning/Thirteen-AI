using Game.Thirteen;
using System.Collections.Generic;

namespace Game.Casino
{
    /// <summary>
    /// Class contains combination that contains multiple same value cards
    /// Ex: Pair, Three of kind, Four of kind and even High card (One of kind)
    /// </summary>
    public class NumberOfKindCardCombination : CardCombination
    {
        public int NumberOfKind { get; private set; }

        public NumberOfKindCardCombination(List<ICard> cards, string combinationName, int numberOfKind) 
            : base(cards, combinationName)
        {
            NumberOfKind = numberOfKind;
        }

        public override bool IsValid()
        {
            return IsValid(CombinationCards, NumberOfKind);
        }

        public static bool IsValid(List<ICard> checkingCards, int numberOfCards)
        {
            if (checkingCards == null || checkingCards.Count != numberOfCards || numberOfCards <= 0)
            {
                return false;
            }

            int valueOfAllCard = checkingCards[0].GetCardValue();
            for (int i = 1; i < numberOfCards; i++)
            {
                if (valueOfAllCard != checkingCards[i].GetCardValue())
                {
                    return false;
                }
            }

            if (GroupCardBySuit(checkingCards).Count != numberOfCards)
            {
                return false;
            }

            return true;
        }

        public override int CompareTo(ICardCombination other)
        {
            if (other is NumberOfKindCardCombination otherCom)
            {
                // Compare the largest card, because all value is same
                return GetHighestCard(CombinationCards).CompareTo(GetHighestCard(otherCom.CombinationCards));
            }
            return 1;
        }
    
        public List<ICard> FindNumberKindOfCard(ECardName cardName, int count)
        {
            if (count > 0)
            {
                Dictionary<ECardName, List<ICard>> cardByName = GroupCardByValue(OwnerCards);
                if (cardByName.TryGetValue(cardName, out List<ICard> cards) && cards.Count >= count)
                {
                    return cards.GetRange(0, count);
                }
            }
            return null;
        }
    
        public List<ICard> FirstKindOfCardByNumber(int count)
        {
            if (count > 0)
            {
                Dictionary<ECardName, List<ICard>> cardByName = GroupCardByValue(OwnerCards);
                foreach (KeyValuePair<ECardName, List<ICard>> it in cardByName)
                {
                    if (it.Value.Count >= count)
                    {
                        return it.Value.GetRange(0, count);
                    }
                }
            }
            return null;
        }

        public List<List<ICard>> FindAllKindOfCard()
        {
            List<List<ICard>> result = new List<List<ICard>>();
            Dictionary<ECardName, List<ICard>> cardByName = GroupCardByValue(OwnerCards);
            // Iterate over each group of cards
            foreach (KeyValuePair<ECardName, List<ICard>> pair in cardByName)
            {
                if(pair.Value.Count >= NumberOfKind)
                {
                    // Add each combination to the result
                    result.Add(pair.Value.GetRange(0, NumberOfKind));
                }
            }

            return result;
        }
    }


    /// <summary>
    /// Class is used to check if 2 NumberOfKindCardCombination can compare to each other
    /// </summary>
    public class NumberOfKindComparableValidator : ICardCombinationComparableValidator
    {
        public bool IsComparable(ICardCombination a, ICardCombination b)
        {
            if (a == null || a is not NumberOfKindCardCombination aCom || b == null || b is not NumberOfKindCardCombination bCom)
            {
                return false;
            }

            if (!aCom.IsValid() || !bCom.IsValid())
            {
                return false;
            }

            if (aCom.NumberOfKind != bCom.NumberOfKind)
            {
                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// The comparer is used to compare 2 NumberOfKindCardCombination
    /// </summary>
    public class NumberOfKindComparer : CardCombinationComparer
    {
        public NumberOfKindComparer()
        {
            ComparableValidator = new NumberOfKindComparableValidator();
        }
    }
}

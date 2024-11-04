
namespace Game.Casino
{
	using System.Collections.Generic;

	public abstract class CardCombination : ICardCombination
	{
		public List<ICard> OwnerCards { get; protected set; }
		public List<ICard> CombinationCards { get; protected set; }
		public string CombinationName { get; private set; }

		public CardCombination(List<ICard> cards, string combinationName)
		{			
			if (cards != null)
			{
				cards.Sort();
			}
			SetInitalCards(cards);
			CombinationName = combinationName;
		}

		/// <summary>
		/// This method should be called after checking the validation of the combination
		/// Otherwise an unexpected behaivour can occur
		/// </summary>
		/// <param name="other"></param>
		/// <returns>integer the represent for the result of comparision</returns>
		public abstract int CompareTo(ICardCombination other);

        /// <summary>
        /// Check to see if the combination is valid
        /// </summary>
        /// <returns></returns>
        public abstract bool IsValid();
		
		/// <summary>
		/// Set the combination back to null
		/// </summary>
		public void Reset()
		{			
			CombinationCards = null;
		}

		public override string ToString()
		{
			string outText = null;
			if (OwnerCards != null)
			{
				outText += $"OwnerCards\n{string.Join("|", OwnerCards)}";
			}
			if (CombinationCards != null)
            {
				outText += $"\n{CombinationName}\n{string.Join("|", CombinationCards)}";
			}
			return outText;
		}

		/// <summary>
		/// Set combination after you are done your filter or choosen
		/// The method will check the legalization of the combination and set it back to null if its illegal
		/// </summary>
		/// <param name="combination">List of card that make a combination</param>
		public void SetCombination(List<ICard> combination)
        {
			CombinationCards = combination;
			// Re-check after setting the combination
			if (!IsValid())
            {
				// its invalid, set it back to null
				CombinationCards = null;
            }
        }

		public void SetInitalCards(List<ICard> initCards)
        {
			OwnerCards = initCards;
        }

		public static ICard GetHighestCard(List<ICard> cards)
        {
			if (cards != null && cards.Count > 0)
            {
				ICard highest = cards[0];
                for (int i = 1; i < cards.Count; i++)
                {
					if (highest.CompareTo(cards[i]) < 0)
                    {
						highest = cards[i];
                    }
                }
				return highest;
            }
			return null;
        }

		public static ICard GetSmallestCard(List<ICard> cards)
		{
			if (cards != null && cards.Count > 0)
			{
				ICard smallest = cards[0];
				for (int i = 1; i < cards.Count; i++)
				{
					if (smallest.CompareTo(cards[i]) > 0)
					{
						smallest = cards[i];
					}
				}
				return smallest;
			}
			return null;
		}

		public static Dictionary<ECardName, List<ICard>> GroupCardByValue(List<ICard> cards)
		{
			// Group the cards by its value
			Dictionary<ECardName, List<ICard>> cardsByValueDict = new Dictionary<ECardName, List<ICard>>();
			for (int i = 0; i < cards.Count; i++)
			{
				ICard card = cards[i];
				if (cardsByValueDict.ContainsKey(card.CardName))
				{
					cardsByValueDict[card.CardName].Add(card);
				}
				else
				{
					cardsByValueDict.Add(card.CardName, new List<ICard>() { card });
				}
			}
			return cardsByValueDict;
		}
		
		public static Dictionary<EFrenchSuit, List<ICard>> GroupCardBySuit(List<ICard> cards)
		{
			Dictionary<EFrenchSuit, List<ICard>> cardsBySuit = new Dictionary<EFrenchSuit, List<ICard>>();
			for (int i = 0; i < cards.Count; i++)
			{
				ICard card = cards[i];
				if (cardsBySuit.ContainsKey(card.Suit))
				{
					cardsBySuit[card.Suit].Add(card);
				}
				else
				{
					cardsBySuit.Add(card.Suit, new List<ICard>() { card });
				}
			}
			return cardsBySuit;
		}
	
		public static List<ICard> GetCardsDistinctValue(List<ICard> inCards)
		{
			List<ICard> outCard = new List<ICard>();
			for (int i = 0; i < inCards.Count; i++)
			{
				ICard checkingCard = inCards[i];
				if (outCard.FindIndex(c => c.GetCardValue() == checkingCard.GetCardValue()) < 0)
				{
					outCard.Add(checkingCard);
				}
			}			
			return outCard;
		}

		public static int GetScore(List<ICard> inCards)
        {
			int score = 0;
            for (int i = 0; i < inCards.Count; i++)
            {
				ICard card = inCards[i];
				int cardScore = card.GetCardValue() * 4 + card.GetSuitValue();
				score += cardScore;
            }

			return score;
        }
	}
}

namespace Game.Thirteen
{
    using Game.Casino;
    using Game.Core;
    using Game.Core.Logger;
    using System.Collections.Generic;

    public class ThirteenCardManager
    {
        public List<ICard> OwnedCards = null;
        public CardCombinationManagement CombinationManagement { get; private set; }

        public ThirteenCardManager()
        {
            CombinationManagement = new CardCombinationManagement();
        }

        public void SetOwnedCard(List<ICard> ownerCards)
        {
            OwnedCards = ownerCards;
            OwnedCards.Sort();
            CombinationManagement.SetCardList(OwnedCards.ConvertAll(c => c.CardId));
        }

        public ICard GetSmallestCard()
        {
            if (!OwnedCards.IsEmpty())
            {
                return OwnedCards[0];
            }
            return default;
        }

        public bool IsOwned(List<ICard> cards)
        {
            if (cards == null || cards.Count == 0)
            {
                return false;
            }

            for (int i = 0; i < cards.Count; i++)
            {
                byte cardId = cards[i].CardId;
                if (OwnedCards.FindIndex(c => c.CardId == cardId) < 0)
                {
                    return false;
                }
            }
            return true;
        }

        public bool RemoveCard(List<ICard> cards)
        {
            if (IsOwned(cards))
            {
                for (int i = 0; i < cards.Count; i++)
                {
                    byte cardId = cards[i].CardId;
                    int idx = OwnedCards.FindIndex(c => c.CardId == cardId);
                    if (idx >= 0)
                    {
                        OwnedCards.RemoveAt(idx);
                    }
                    CombinationManagement.MarkForDeleteCard(cardId);
                }
                CombinationManagement.DeleteUnusableBuilders();
                return true;
            }
            return false;
        }

        public CardCombination CreateCardCombination(List<byte> cardIds)
        {
            if (cardIds.IsEmpty())
            {
                return null;
            }

            List<ICard> cards = new List<ICard>();
            for (int i = 0; i < cardIds.Count; i++)
            {
                byte cardId = cardIds[i];
                int idx = OwnedCards.FindIndex(c => c.CardId == cardId);
                if (idx >= 0)
                {
                    cards.Add(OwnedCards[idx]);
                }
                else
                {
                    // Not owned the card so return null right away
                    return null;
                }
            }

            return CreateCardCombination(cards);
        }

        public CardCombination FirstAgainstCombinationFor(CardCombination otherCombination)
        {
            for (int i = 0; i < CombinationManagement.Builders.Count; i++)
            {
                CombinationBuilder builder = CombinationManagement.Builders[i];
                CardCombination com = builder.Build();
                if (CardCombinationManagement.CombinationComparer.Compare(otherCombination, com) == ECardCombinationComparisionResult.RightGreater)
                {
                    return com;
                }
            }
            return null;
        }

        public CardCombination FindAgainstCombinationFor(CardCombination combination)
        {
            if (combination.CombinationCards.IsEmpty() || combination.OwnerCards.Count > OwnedCards.Count)
            {
                return null;
            }

            // The other is Hight Card, Pair, Three Of Kind, Four of Kind, Pig or two Pig
            if (combination is NumberOfKindCardCombination otherNumberOfKind)
            {
                // The other is Hight Card, Pair, Three Of Kind or Four of Kind
                NumberOfKindCardCombination set = new NumberOfKindCardCombination(OwnedCards, otherNumberOfKind.CombinationName, otherNumberOfKind.NumberOfKind);
                ICard otherCard = combination.CombinationCards[0];
                List<ICard> myCard = set.FindNumberKindOfCard(otherCard.CardName, otherNumberOfKind.NumberOfKind);
                if (!myCard.IsEmpty())
                {
                    // This number of kind cards might greater or not so have to check it
                    CardCombination numberOfKind = CreateCardCombination(myCard);
                    if (otherNumberOfKind.CompareTo(numberOfKind) < 0)
                    {
                        return numberOfKind;
                    }
                }

                // Keep finding the greater number of kind cards.
                Dictionary<ECardName, List<ICard>> cardByName = CardCombination.GroupCardByValue(OwnedCards);
                List<ECardName> startCardNames = new List<ECardName>()
                {
                    ECardName.Three,
                    ECardName.Four,
                    ECardName.Five,
                    ECardName.Six,
                    ECardName.Seven,
                    ECardName.Eight,
                    ECardName.Nine,
                    ECardName.Ten,
                    ECardName.J,
                    ECardName.Q,
                    ECardName.K,
                    ECardName.A,
                    ECardName.Two,
                };

                int currentCardNameIndex = startCardNames.FindIndex(n => n == otherCard.CardName);
                if (currentCardNameIndex >= 0)
                {
                    currentCardNameIndex++;
                    while (currentCardNameIndex < startCardNames.Count)
                    {
                        if (cardByName.TryGetValue(startCardNames[currentCardNameIndex], out List<ICard> cards))
                        {
                            if (cards.Count >= otherNumberOfKind.NumberOfKind)
                            {
                                List<ICard> numberOfKindCards = cards.GetRange(0, otherNumberOfKind.NumberOfKind);
                                if (!numberOfKindCards.IsEmpty())
                                {
                                    return CreateCardCombination(numberOfKindCards);
                                }
                            }
                        }

                        currentCardNameIndex++;
                    }
                }

                // Other is Two or Pair of Two                
                ThirteenContinuouslyPairsCardCombination continuousPairs = new ThirteenContinuouslyPairsCardCombination(OwnedCards);
                if (otherCard.CardName == ECardName.Two && otherNumberOfKind.CombinationCards.Count <= 2)
                {
                    // only one Pig
                    if (otherNumberOfKind.NumberOfKind == 1)
                    {
                        List<ICard> threePairs = continuousPairs.FirstContinouslyPairValues(3);
                        if (!threePairs.IsEmpty())
                        {
                            return CreateCardCombination(threePairs);
                        }
                    }

                    // two Pig
                    if (otherNumberOfKind.NumberOfKind <= 2)
                    {
                        List<ICard> fourOfKind = new ThirteenFourOfKindCardCombination(OwnedCards).FirstKindOfCardByNumber(4);
                        if (!fourOfKind.IsEmpty())
                        {
                            return CreateCardCombination(fourOfKind);
                        }

                        List<ICard> fourPairs = continuousPairs.FirstContinouslyPairValues(4);
                        if (!fourPairs.IsEmpty())
                        {
                            return CreateCardCombination(fourPairs);
                        }
                    }
                }

                // Other is 4 kind of card
                if (combination is ThirteenFourOfKindCardCombination)
                {
                    List<ICard> fourPairs = continuousPairs.FirstContinouslyPairValues(4);
                    if (!fourPairs.IsEmpty())
                    {
                        return CreateCardCombination(fourPairs);
                    }
                }
            }

            // Other is continously card
            if (combination is ThirteenContinuouslyPairsCardCombination otherContinouslyPairs)
            {
                int otherPairs = otherContinouslyPairs.NumberOfKind / 2;
                ThirteenContinuouslyPairsCardCombination continuousPairs = new ThirteenContinuouslyPairsCardCombination(OwnedCards);

                if (otherPairs == 3)
                {
                    // Find greater 3 pairs
                    ICard smallestCard = CardCombination.GetSmallestCard(otherContinouslyPairs.OwnerCards);
                    List<ICard> threePairs = continuousPairs.GetContinouslyPairValues(new ThirteenContinuouslyPairsCardCombination.Filter()
                    {
                        NumberOfPairs = otherPairs,
                        StartByValue = smallestCard.GetCardValue(),
                    });

                    // This three pairs might greater or not so have to check it
                    if (!threePairs.IsEmpty())
                    {
                        CardCombination threePairsCombination = CreateCardCombination(threePairs);
                        if (otherContinouslyPairs.CompareTo(threePairsCombination) < 0)
                        {
                            return threePairsCombination;
                        }
                    }

                    // Keep finding the greater three pairs
                    List<ECardName> startCardNames = new List<ECardName>()
                    {
                        ECardName.Three,
                        ECardName.Four,
                        ECardName.Five,
                        ECardName.Six,
                        ECardName.Seven,
                        ECardName.Eight,
                        ECardName.Nine,
                        ECardName.Ten,
                        ECardName.J,
                        ECardName.Q,
                    };

                    int currentCardNameIndex = startCardNames.FindIndex(n => n == smallestCard.CardName);
                    if (currentCardNameIndex >= 0)
                    {
                        currentCardNameIndex++;
                        while (currentCardNameIndex < startCardNames.Count)
                        {
                            CasinoCard tempCard = new CasinoCard(EFrenchSuit.Spade, startCardNames[currentCardNameIndex]);
                            threePairs = continuousPairs.GetContinouslyPairValues(new ThirteenContinuouslyPairsCardCombination.Filter()
                            {
                                NumberOfPairs = otherPairs,
                                StartByValue = tempCard.GetCardValue(),
                            });

                            if (!threePairs.IsEmpty())
                            {
                                return CreateCardCombination(threePairs);
                            }

                            currentCardNameIndex++;
                        }
                    }

                    // Find four of kind
                    List<ICard> fourKindOfCard = continuousPairs.FirstKindOfCardByNumber(4);
                    if (!fourKindOfCard.IsEmpty())
                    {
                        return CreateCardCombination(fourKindOfCard);
                    }

                    // Find four pairs
                    List<ICard> fourPairs = continuousPairs.FirstContinouslyPairValues(4);
                    if (!fourPairs.IsEmpty())
                    {
                        return CreateCardCombination(fourPairs);
                    }
                }


                if (otherPairs == 4)
                {
                    // Find greater four pairs
                    ICard smallestCard = CardCombination.GetSmallestCard(otherContinouslyPairs.OwnerCards);
                    List<ICard> fourPairs = continuousPairs.GetContinouslyPairValues(new ThirteenContinuouslyPairsCardCombination.Filter()
                    {
                        NumberOfPairs = otherPairs,
                        StartByValue = smallestCard.GetCardValue(),
                    });

                    if (!fourPairs.IsEmpty())
                    {
                        CardCombination fourPairsCombination = CreateCardCombination(fourPairs);
                        if (otherContinouslyPairs.CompareTo(fourPairsCombination) < 0)
                        {
                            return fourPairsCombination;
                        }
                    }

                    // Keep finding the greater three pairs
                    List<ECardName> startCardNames = new List<ECardName>()
                    {
                        ECardName.Three,
                        ECardName.Four,
                        ECardName.Five,
                        ECardName.Six,
                        ECardName.Seven,
                        ECardName.Eight,
                        ECardName.Nine,
                        ECardName.Ten,
                        ECardName.J,
                    };
                    int currentCardNameIndex = startCardNames.FindIndex(n => n == smallestCard.CardName);
                    if (currentCardNameIndex >= 0)
                    {
                        currentCardNameIndex++;
                        while (currentCardNameIndex < startCardNames.Count)
                        {
                            CasinoCard tempCard = new CasinoCard(EFrenchSuit.Spade, startCardNames[currentCardNameIndex]);
                            fourPairs = continuousPairs.GetContinouslyPairValues(new ThirteenContinuouslyPairsCardCombination.Filter()
                            {
                                NumberOfPairs = otherPairs,
                                StartByValue = tempCard.GetCardValue(),
                            });

                            if (!fourPairs.IsEmpty())
                            {
                                return CreateCardCombination(fourPairs);
                            }

                            currentCardNameIndex++;
                        }
                    }
                }
            }

            // Find greater straight
            if (combination is ThirteenStraightCombination otherStraight)
            {
                List<ICard> cloneCards = new List<ICard>(OwnedCards);
                cloneCards.RemoveAll(c => c.CardName == ECardName.Two);

                ThirteenStraightCombination straight = new ThirteenStraightCombination(cloneCards);
                ICard smallestCard = CardCombination.GetSmallestCard(otherStraight.CombinationCards);
                int straightNumberOfCard = CardCombination.GetCardsDistinctValue(otherStraight.CombinationCards).Count;
                List<ICard> straightCards = straight.Lookup(new ThirteenStraightCombination.Filter()
                {
                    NumberOfCard = straightNumberOfCard,
                    StartByValue = smallestCard.GetCardValue(),
                    SuitsValidForTheLastCard = EFrenchSuit.Club | EFrenchSuit.Spade | EFrenchSuit.Diamond | EFrenchSuit.Heart
                });

                // This three pairs might greater or not so have to check it
                if (!straightCards.IsEmpty())
                {
                    CardCombination straightCombination = CreateCardCombination(straightCards);
                    if (straightCombination is ThirteenStraightCombination tempStraight)
                    {
                        if (tempStraight.IsValid() && otherStraight.CompareTo(tempStraight) < 0)
                        {
                            return straightCombination;
                        }
                    }
                }

                // Keep finding the greater three pairs
                List<ECardName> startCardNames = new List<ECardName>()
                {
                    ECardName.Three,
                    ECardName.Four,
                    ECardName.Five,
                    ECardName.Six,
                    ECardName.Seven,
                    ECardName.Eight,
                    ECardName.Nine,
                    ECardName.Ten,
                    ECardName.J,
                    ECardName.Q,
                };

                int currentCardNameIndex = startCardNames.FindIndex(n => n == smallestCard.CardName);
                if (currentCardNameIndex >= 0)
                {
                    currentCardNameIndex++;
                    while (currentCardNameIndex < startCardNames.Count)
                    {
                        CasinoCard tempCard = new CasinoCard(EFrenchSuit.Spade, startCardNames[currentCardNameIndex]);
                        straightCards = straight.Lookup(new ThirteenStraightCombination.Filter()
                        {
                            NumberOfCard = straightNumberOfCard,
                            StartByValue = tempCard.GetCardValue(),
                            SuitsValidForTheLastCard = EFrenchSuit.Club | EFrenchSuit.Spade | EFrenchSuit.Diamond | EFrenchSuit.Heart
                        });

                        if (!straightCards.IsEmpty())
                        {
                            CardCombination straightCombination = CreateCardCombination(straightCards);
                            if (straightCombination is ThirteenStraightCombination tempStraight)
                            {
                                if (tempStraight.IsValid() && otherStraight.CompareTo(tempStraight) < 0)
                                {
                                    return straightCombination;
                                }
                            }
                        }

                        currentCardNameIndex++;
                    }
                }
            }

            return null;
        }

        public List<CardCombination> FindAllCombination()
        {
            if (this.OwnedCards.IsEmpty())
            {
                return null;
            }
            List<CardCombination> results = new List<CardCombination>();
            // Continuous Pairs
            ThirteenContinuouslyPairsCardCombination continuousPairs = new ThirteenContinuouslyPairsCardCombination(OwnedCards);
            // Four Continuous Pairs
            List<ICard> fourPairCards = continuousPairs.FirstContinouslyPairValues(4);
            if (!fourPairCards.IsEmpty())
            {
                CardCombination cardCombination = CreateCardCombination(fourPairCards);
                if (cardCombination != null)
                {
                    results.Add(cardCombination);
                }
            }

            // Three Continuous Pairs
            List<ICard> threePairCards = continuousPairs.FirstContinouslyPairValues(3);
            if (!threePairCards.IsEmpty())
            {
                CardCombination cardCombination = CreateCardCombination(threePairCards);
                if (cardCombination != null)
                {
                    results.Add(cardCombination);
                }
            }

            // Four Of Kind Card Combination
            ThirteenFourOfKindCardCombination fourOfKind = new ThirteenFourOfKindCardCombination(OwnedCards);
            List<List<ICard>> fourOfKindCards = fourOfKind.FindAllKindOfCard();
            for (int i = 0; i < fourOfKindCards.Count; i++)
            {
                List<ICard> tempCards = fourOfKindCards[i];
                if (!tempCards.IsEmpty())
                {
                    CardCombination cardCombination = CreateCardCombination(tempCards);
                    if (cardCombination != null)
                    {
                        results.Add(cardCombination);
                    }
                }
            }

            // Three Of Kind Card Combination
            ThirteenThreeOfKindCardCombination threeOfKind = new ThirteenThreeOfKindCardCombination(OwnedCards);
            List<List<ICard>> threeOfKindCards = threeOfKind.FindAllKindOfCard();
            for (int i = 0; i < threeOfKindCards.Count; i++)
            {
                List<ICard> tempCards = threeOfKindCards[i];
                if (!tempCards.IsEmpty())
                {
                    CardCombination cardCombination = CreateCardCombination(tempCards);
                    if (cardCombination != null)
                    {
                        results.Add(cardCombination);
                    }
                }
            }

            // Straight Combination
            List<ICard> cloneCards = new List<ICard>(OwnedCards);
            cloneCards.RemoveAll(c => c.CardName == ECardName.Two);
            ThirteenStraightCombination straight = new ThirteenStraightCombination(cloneCards);
            ICard smallestCard = CardCombination.GetSmallestCard(cloneCards);
            int maxNumberOfCard = CardCombination.GetCardsDistinctValue(cloneCards).Count;
            int minNumberOfCard = 3;
            List<ECardName> startCardNames = new List<ECardName>()
            {
                ECardName.Three,
                ECardName.Four,
                ECardName.Five,
                ECardName.Six,
                ECardName.Seven,
                ECardName.Eight,
                ECardName.Nine,
                ECardName.Ten,
                ECardName.J,
                ECardName.Q,
            };
            if (smallestCard != null)
            {
                int currentCardNameIndex = startCardNames.FindIndex(n => n == smallestCard.CardName);
                if (currentCardNameIndex >= 0)
                {
                    while (currentCardNameIndex < startCardNames.Count)
                    {
                        while (minNumberOfCard < maxNumberOfCard)
                        {
                            CasinoCard tempCard = new CasinoCard(EFrenchSuit.Spade, startCardNames[currentCardNameIndex]);
                            List<ICard> straightCards = straight.Lookup(new ThirteenStraightCombination.Filter()
                            {
                                NumberOfCard = minNumberOfCard,
                                StartByValue = tempCard.GetCardValue(),
                                SuitsValidForTheLastCard = EFrenchSuit.Club | EFrenchSuit.Spade | EFrenchSuit.Diamond | EFrenchSuit.Heart
                            });

                            if (!straightCards.IsEmpty())
                            {
                                CardCombination cardCombination = CreateCardCombination(straightCards);
                                if (cardCombination != null)
                                {
                                    results.Add(cardCombination);
                                }
                            }
                            minNumberOfCard++;
                        }

                        // Reset Straight Number Of Card in new star positions.
                        minNumberOfCard = 3;
                        currentCardNameIndex++;
                    }
                }
            }

            // Pair Combination
            ThirteenPairCombination pair = new ThirteenPairCombination(OwnedCards);
            List<List<ICard>> pairCards = pair.FindAllKindOfCard();
            for (int i = 0; i < pairCards.Count; i++)
            {
                List<ICard> tempCards = pairCards[i];
                if (!tempCards.IsEmpty())
                {
                    CardCombination cardCombination = CreateCardCombination(tempCards);
                    if (cardCombination != null)
                    {
                        results.Add(cardCombination);
                    }
                }
            }

            // High Card Combination
            ThirteenHighCardCombination highCard = new ThirteenHighCardCombination(OwnedCards);
            List<List<ICard>> highCards = highCard.FindAllKindOfCard();
            for (int i = 0; i < highCards.Count; i++)
            {
                List<ICard> tempCards = highCards[i];
                if (!tempCards.IsEmpty())
                {
                    CardCombination cardCombination = CreateCardCombination(tempCards);
                    if (cardCombination != null)
                    {
                        results.Add(cardCombination);
                    }
                }
            }

            return results;
        }

        public Dictionary<string, List<CardCombination>> GetAllCardCombination()
        {
            List<ICard> cloneCards = new List<ICard>(OwnedCards);
            Dictionary<string, List<CardCombination>> results = new Dictionary<string, List<CardCombination>>();

            // Continuous Pairs
            ThirteenContinuouslyPairsCardCombination continuousPairs = new ThirteenContinuouslyPairsCardCombination(cloneCards);

            List<ICard> fourPairCards = continuousPairs.FirstContinouslyPairValues(4);
            if (!fourPairCards.IsEmpty())
            {
                if (!results.ContainsKey(continuousPairs.CombinationName))
                {
                    results.Add(continuousPairs.CombinationName, new List<CardCombination>());
                }
                results[continuousPairs.CombinationName].Add(CreateCardCombination(fourPairCards));
                cloneCards.RemoveAll(c => fourPairCards.Contains(c));
            }

            List<ICard> threePairCards = continuousPairs.FirstContinouslyPairValues(3);
            if (!threePairCards.IsEmpty())
            {
                if (!results.ContainsKey(continuousPairs.CombinationName))
                {
                    results.Add(continuousPairs.CombinationName, new List<CardCombination>());
                }
                results[continuousPairs.CombinationName].Add(CreateCardCombination(threePairCards));
                cloneCards.RemoveAll(c => threePairCards.Contains(c));
            }
            // End Continuous Pairs

            // Four of kind.
            ThirteenFourOfKindCardCombination fourOfKind = new ThirteenFourOfKindCardCombination(cloneCards);
            Dictionary<ECardName, List<ICard>> fourOfKindCardByName = CardCombination.GroupCardByValue(cloneCards);
            foreach (KeyValuePair<ECardName, List<ICard>> it in fourOfKindCardByName)
            {
                if (it.Value.Count >= 4)
                {
                    List<ICard> fourOfKindCards = it.Value.GetRange(0, 4);
                    if (!results.ContainsKey(fourOfKind.CombinationName))
                    {
                        results.Add(fourOfKind.CombinationName, new List<CardCombination>());
                    }
                    results[fourOfKind.CombinationName].Add(CreateCardCombination(fourOfKindCards));
                    cloneCards.RemoveAll(c => fourOfKindCards.Contains(c));
                }
            }

            // End Four of kind.

            // Straight
            ThirteenStraightCombination straight = new ThirteenStraightCombination(cloneCards);
            List<List<ICard>> straightCards = straight.FindAllStraightCard();
            for (int i = 0; i < straightCards.Count; i++)
            {
                List<ICard> cards = straightCards[i];
                if (!cards.IsEmpty())
                {
                    if (!results.ContainsKey(straight.CombinationName))
                    {
                        results.Add(straight.CombinationName, new List<CardCombination>());
                    }

                    CardCombination straightCombination = CreateCardCombination(cards);
                    if (straightCombination != null && straightCombination.CombinationCards.Count > 0)
                    {
                        results[straight.CombinationName].Add(straightCombination);
                        cloneCards.RemoveAll(c => cards.Contains(c));
                    }
                }
            }
            // End Straight.

            // Three of kind.
            ThirteenThreeOfKindCardCombination threeOfKind = new ThirteenThreeOfKindCardCombination(cloneCards);
            Dictionary<ECardName, List<ICard>> threeOfKindCardByName = CardCombination.GroupCardByValue(cloneCards);
            foreach (KeyValuePair<ECardName, List<ICard>> it in threeOfKindCardByName)
            {
                if (it.Value.Count == 3)
                {
                    List<ICard> threeOfKindCards = it.Value.GetRange(0, 3);
                    if (!results.ContainsKey(threeOfKind.CombinationName))
                    {
                        results.Add(threeOfKind.CombinationName, new List<CardCombination>());
                    }
                    results[threeOfKind.CombinationName].Add(CreateCardCombination(threeOfKindCards));
                    cloneCards.RemoveAll(c => threeOfKindCards.Contains(c));
                }
            }
            // End Three of kind.

            // Pair.
            ThirteenPairCombination pair = new ThirteenPairCombination(cloneCards);
            foreach (KeyValuePair<ECardName, List<ICard>> item in pair.Pairs)
            {
                if (!results.ContainsKey(pair.CombinationName))
                {
                    results.Add(pair.CombinationName, new List<CardCombination>());
                }

                results[pair.CombinationName].Add(CreateCardCombination(item.Value));
                cloneCards.RemoveAll(c => item.Value.Contains(c));
            }
            // End Pair.

            // Single
            Dictionary<ECardName, List<ICard>> hightCardByName = CardCombination.GroupCardByValue(cloneCards);
            ThirteenHighCardCombination highCard = new ThirteenHighCardCombination(cloneCards);
            foreach (KeyValuePair<ECardName, List<ICard>> it in hightCardByName)
            {
                if (it.Value.Count == 1)
                {
                    List<ICard> highCards = it.Value.GetRange(0, 1);
                    if (!results.ContainsKey(highCard.CombinationName))
                    {
                        results.Add(highCard.CombinationName, new List<CardCombination>());
                    }
                    results[highCard.CombinationName].Add(CreateCardCombination(highCards));
                    cloneCards.RemoveAll(c => highCards.Contains(c));
                }
            }

            // End Single.
            return results;
        }

        public bool GetBestAttackCardStrategies(int opponentCardsRemaining, out CardCombination cardCombination)
        {
            switch (opponentCardsRemaining)
            {
                case 1:
                    {
                        // If AI has a pair, play smallest pair.
                        ThirteenPairCombination pair = new ThirteenPairCombination(OwnedCards);
                        List<ICard> pairCards = pair.GetSmallestPair();
                        if (pairCards.IsEmpty())
                        {
                            cardCombination = CreateCardCombination(pairCards);
                            return true;
                        }
                        // End Pair.

                        // If AI has a straight, play it. 
                        // The 2 cannot be inside the Straight and Continuously Pairs
                        List<ICard> cloneCards = new List<ICard>(OwnedCards);
                        cloneCards.RemoveAll(c => c.CardName == ECardName.Two);

                        ThirteenStraightCombination straight = new ThirteenStraightCombination(cloneCards);
                        ICard smallestCard = CardCombination.GetSmallestCard(straight.OwnerCards);
                        if (smallestCard != null)
                        {
                            List<ICard> straightCards = new List<ICard>();
                            List<ECardName> startCardNames = new List<ECardName>()
                            {
                                ECardName.Three,
                                ECardName.Four,
                                ECardName.Five,
                                ECardName.Six,
                                ECardName.Seven,
                                ECardName.Eight,
                                ECardName.Nine,
                                ECardName.Ten,
                                ECardName.J,
                                ECardName.Q,
                            };

                            int currentCardNameIndex = startCardNames.FindIndex(n => n == smallestCard.CardName);
                            if (currentCardNameIndex >= 0)
                            {
                                currentCardNameIndex++;
                                while (currentCardNameIndex < startCardNames.Count)
                                {
                                    CasinoCard tempCard = new CasinoCard(EFrenchSuit.Spade, startCardNames[currentCardNameIndex]);
                                    straightCards = straight.Lookup(new ThirteenStraightCombination.Filter()
                                    {
                                        NumberOfCard = CardCombination.GetCardsDistinctValue(straight.OwnerCards).Count,
                                        StartByValue = tempCard.GetCardValue(),
                                        SuitsValidForTheLastCard = EFrenchSuit.Club | EFrenchSuit.Spade | EFrenchSuit.Diamond | EFrenchSuit.Heart
                                    });

                                    if (!straightCards.IsEmpty())
                                    {
                                        cardCombination = CreateCardCombination(straightCards);
                                        return true;
                                    }

                                    currentCardNameIndex++;
                                }
                            }
                        }
                        // End Straight.

                        // If AI has a single, play high card.
                        ThirteenHighCardCombination highCard = new ThirteenHighCardCombination(OwnedCards);
                        ICard card = CardCombination.GetHighestCard(highCard.OwnerCards);
                        if (card != null)
                        {
                            cardCombination = CreateCardCombination(new List<byte>() { card.CardId });
                            return true;
                        }
                        // End Single.
                    }
                    break;
                case 2:
                    {
                        // If AI has a straight, play it. 
                        // The 2 cannot be inside the Straight and Continuously Pairs
                        List<ICard> cloneCards = new List<ICard>(OwnedCards);
                        cloneCards.RemoveAll(c => c.CardName == ECardName.Two);

                        ThirteenStraightCombination straight = new ThirteenStraightCombination(cloneCards);
                        ICard smallestCard = CardCombination.GetSmallestCard(straight.OwnerCards);
                        if (smallestCard != null)
                        {
                            List<ICard> straightCards = new List<ICard>();
                            List<ECardName> startCardNames = new List<ECardName>()
                            {
                                ECardName.Three,
                                ECardName.Four,
                                ECardName.Five,
                                ECardName.Six,
                                ECardName.Seven,
                                ECardName.Eight,
                                ECardName.Nine,
                                ECardName.Ten,
                                ECardName.J,
                                ECardName.Q,
                            };

                            int currentCardNameIndex = startCardNames.FindIndex(n => n == smallestCard.CardName);
                            if (currentCardNameIndex >= 0)
                            {
                                currentCardNameIndex++;
                                while (currentCardNameIndex < startCardNames.Count)
                                {
                                    CasinoCard tempCard = new CasinoCard(EFrenchSuit.Spade, startCardNames[currentCardNameIndex]);
                                    straightCards = straight.Lookup(new ThirteenStraightCombination.Filter()
                                    {
                                        NumberOfCard = CardCombination.GetCardsDistinctValue(straight.OwnerCards).Count,
                                        StartByValue = tempCard.GetCardValue(),
                                        SuitsValidForTheLastCard = EFrenchSuit.Club | EFrenchSuit.Spade | EFrenchSuit.Diamond | EFrenchSuit.Heart
                                    });

                                    if (!straightCards.IsEmpty())
                                    {
                                        cardCombination = CreateCardCombination(straightCards);
                                        return true;
                                    }

                                    currentCardNameIndex++;
                                }
                            }
                        }
                        // End Straight.

                        // If AI has a three of kind, play it.  
                        ThirteenThreeOfKindCardCombination threeOfKind = new ThirteenThreeOfKindCardCombination(OwnedCards);
                        List<ICard> threeOfKindCards = threeOfKind.FirstKindOfCardByNumber(3);
                        if (!threeOfKindCards.IsEmpty())
                        {
                            cardCombination = CreateCardCombination(threeOfKindCards);
                            return true;
                        }
                        // End Three Of Kind

                        // If AI has a single, play smallest card.
                        ThirteenHighCardCombination highCard = new ThirteenHighCardCombination(OwnedCards);
                        ICard card = CardCombination.GetSmallestCard(highCard.OwnerCards);
                        if (card != null)
                        {
                            cardCombination = CreateCardCombination(new List<byte>() { card.CardId });
                            return true;
                        }
                        // End Single.

                        // If AI has a pair, play hightest pair.
                        ThirteenPairCombination pair = new ThirteenPairCombination(OwnedCards);
                        List<ICard> pairCards = pair.GetHighestPair();
                        if (pairCards.IsEmpty())
                        {
                            cardCombination = CreateCardCombination(pairCards);
                            return true;
                        }
                        // End Pair.
                    }
                    break;
                case 3:
                    {
                        // If AI has a pair, play smallest pair.
                        ThirteenPairCombination pair = new ThirteenPairCombination(OwnedCards);
                        List<ICard> pairCards = pair.GetSmallestPair();
                        if (pairCards.IsEmpty())
                        {
                            cardCombination = CreateCardCombination(pairCards);
                            return true;
                        }
                        // End Pair.

                        // If AI has a straight, play it. 
                        // The 2 cannot be inside the Straight and Continuously Pairs
                        List<ICard> cloneCards = new List<ICard>(OwnedCards);
                        cloneCards.RemoveAll(c => c.CardName == ECardName.Two);

                        ThirteenStraightCombination straight = new ThirteenStraightCombination(cloneCards);
                        ICard smallestCard = CardCombination.GetSmallestCard(straight.OwnerCards);
                        if (smallestCard != null)
                        {
                            List<ICard> straightCards = new List<ICard>();
                            List<ECardName> startCardNames = new List<ECardName>()
                            {
                                ECardName.Three,
                                ECardName.Four,
                                ECardName.Five,
                                ECardName.Six,
                                ECardName.Seven,
                                ECardName.Eight,
                                ECardName.Nine,
                                ECardName.Ten,
                                ECardName.J,
                                ECardName.Q,
                            };

                            int currentCardNameIndex = startCardNames.FindIndex(n => n == smallestCard.CardName);
                            if (currentCardNameIndex >= 0)
                            {
                                currentCardNameIndex++;
                                while (currentCardNameIndex < startCardNames.Count)
                                {
                                    CasinoCard tempCard = new CasinoCard(EFrenchSuit.Spade, startCardNames[currentCardNameIndex]);
                                    straightCards = straight.Lookup(new ThirteenStraightCombination.Filter()
                                    {
                                        NumberOfCard = CardCombination.GetCardsDistinctValue(straight.OwnerCards).Count,
                                        StartByValue = tempCard.GetCardValue(),
                                        SuitsValidForTheLastCard = EFrenchSuit.Club | EFrenchSuit.Spade | EFrenchSuit.Diamond | EFrenchSuit.Heart
                                    });

                                    if (!straightCards.IsEmpty())
                                    {
                                        cardCombination = CreateCardCombination(straightCards);
                                        return true;
                                    }

                                    currentCardNameIndex++;
                                }
                            }
                        }
                        // End Straight.

                        // If AI has a single, play smallest card.
                        ThirteenHighCardCombination highCard = new ThirteenHighCardCombination(OwnedCards);
                        ICard card = CardCombination.GetSmallestCard(highCard.OwnerCards);
                        if (card != null)
                        {
                            cardCombination = CreateCardCombination(new List<byte>() { card.CardId });
                            return true;
                        }
                        // End Single.

                        // If AI has a three of kind, play it.
                        ThirteenThreeOfKindCardCombination threeOfKind = new ThirteenThreeOfKindCardCombination(OwnedCards);
                        List<ICard> threeOfKindCards = threeOfKind.FirstKindOfCardByNumber(3);
                        if (!threeOfKindCards.IsEmpty())
                        {
                            cardCombination = CreateCardCombination(threeOfKindCards);
                            return true;
                        }
                        // End Three Of Kind
                    }
                    break;
                case 4:
                    {
                        // If AI has a single, play smallest card.
                        ThirteenHighCardCombination highCard = new ThirteenHighCardCombination(OwnedCards);
                        ICard card = CardCombination.GetSmallestCard(highCard.OwnerCards);
                        if (card != null)
                        {
                            cardCombination = CreateCardCombination(new List<byte>() { card.CardId });
                            return true;
                        }
                        // End Single.

                        // If AI has a pair, play smallest pair.
                        ThirteenPairCombination pair = new ThirteenPairCombination(OwnedCards);
                        List<ICard> pairCards = pair.GetSmallestPair();
                        if (pairCards.IsEmpty())
                        {
                            cardCombination = CreateCardCombination(pairCards);
                            return true;
                        }
                        // End Pair.

                        // If AI has a straight, play it. 
                        // The 2 cannot be inside the Straight and Continuously Pairs
                        List<ICard> cloneCards = new List<ICard>(OwnedCards);
                        cloneCards.RemoveAll(c => c.CardName == ECardName.Two);

                        ThirteenStraightCombination straight = new ThirteenStraightCombination(cloneCards);
                        ICard smallestCard = CardCombination.GetSmallestCard(straight.OwnerCards);
                        if (smallestCard != null)
                        {
                            List<ICard> straightCards = new List<ICard>();
                            List<ECardName> startCardNames = new List<ECardName>()
                            {
                                ECardName.Three,
                                ECardName.Four,
                                ECardName.Five,
                                ECardName.Six,
                                ECardName.Seven,
                                ECardName.Eight,
                                ECardName.Nine,
                                ECardName.Ten,
                                ECardName.J,
                                ECardName.Q,
                            };

                            int currentCardNameIndex = startCardNames.FindIndex(n => n == smallestCard.CardName);
                            if (currentCardNameIndex >= 0)
                            {
                                currentCardNameIndex++;
                                while (currentCardNameIndex < startCardNames.Count)
                                {
                                    CasinoCard tempCard = new CasinoCard(EFrenchSuit.Spade, startCardNames[currentCardNameIndex]);
                                    straightCards = straight.Lookup(new ThirteenStraightCombination.Filter()
                                    {
                                        NumberOfCard = CardCombination.GetCardsDistinctValue(straight.OwnerCards).Count,
                                        StartByValue = tempCard.GetCardValue(),
                                        SuitsValidForTheLastCard = EFrenchSuit.Club | EFrenchSuit.Spade | EFrenchSuit.Diamond | EFrenchSuit.Heart
                                    });

                                    if (!straightCards.IsEmpty())
                                    {
                                        cardCombination = CreateCardCombination(straightCards);
                                        return true;
                                    }

                                    currentCardNameIndex++;
                                }
                            }
                        }
                        // End Straight.

                        // If AI has a three of kind, play it.
                        ThirteenThreeOfKindCardCombination threeOfKind = new ThirteenThreeOfKindCardCombination(OwnedCards);
                        List<ICard> threeOfKindCards = threeOfKind.FirstKindOfCardByNumber(3);
                        if (!threeOfKindCards.IsEmpty())
                        {
                            cardCombination = CreateCardCombination(threeOfKindCards);
                            return true;
                        }
                        // End Three Of Kind
                    }
                    break;
                default:
                    break;
            }

            cardCombination = null;
            return false;
        }

        public static CardCombination CreateCardCombination(List<ICard> selectedCards)
        {
            int numberOfCard = selectedCards.Count;
            selectedCards.Sort();
            switch (numberOfCard)
            {
                case 1:
                    {
                        ThirteenHighCardCombination hightCard = new ThirteenHighCardCombination(selectedCards);
                        hightCard.SetCombination(selectedCards);
                        return hightCard;
                    }
                case 2:
                    {
                        ThirteenPairCombination pair = new ThirteenPairCombination(selectedCards);
                        pair.SetCombination(pair.GetHighestPair());
                        if (pair.IsValid())
                        {
                            return pair;
                        }
                        break;
                    }
                case 3:
                    {
                        ThirteenThreeOfKindCardCombination threeOfKind = new ThirteenThreeOfKindCardCombination(selectedCards);
                        threeOfKind.SetCombination(selectedCards);
                        if (threeOfKind.IsValid())
                        {
                            return threeOfKind;
                        }
                        break;
                    }
                case 4:
                    {
                        ThirteenFourOfKindCardCombination fourOfKind = new ThirteenFourOfKindCardCombination(selectedCards);
                        fourOfKind.SetCombination(selectedCards);
                        if (fourOfKind.IsValid())
                        {
                            return fourOfKind;
                        }
                        break;
                    }
                case 6:
                    {
                        ThirteenContinuouslyPairsCardCombination threeContinouslyPairs = new ThirteenContinuouslyPairsCardCombination(selectedCards);
                        threeContinouslyPairs.SetCombination(selectedCards);
                        List<ICard> threeContinouslyPairCards = threeContinouslyPairs.FirstContinouslyPairValues(3);
                        if (threeContinouslyPairCards != null && threeContinouslyPairCards.Count > 0)
                        {
                            return threeContinouslyPairs;
                        }
                        break;
                    }
                case 8:
                    {
                        ThirteenContinuouslyPairsCardCombination fourContinouslyPairs = new ThirteenContinuouslyPairsCardCombination(selectedCards);
                        fourContinouslyPairs.SetCombination(selectedCards);
                        List<ICard> fourContinouslyPairCards = fourContinouslyPairs.FirstContinouslyPairValues(4);
                        if (fourContinouslyPairCards != null && fourContinouslyPairCards.Count > 0)
                        {
                            return fourContinouslyPairs;
                        }
                        break;
                    }
                default:
                    break;
            }

            ThirteenStraightCombination straight = new ThirteenStraightCombination(selectedCards);
            straight.SetCombination(selectedCards);
            if (straight.IsValid())
            {
                return straight;
            }
            return null;
        }
    }
}

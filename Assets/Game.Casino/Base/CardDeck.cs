namespace Game.Casino
{
    using Game.Core;
    using Game.Core.Logger;
    using Game.Core.Utilities;
    using System.Collections.Generic;

    public struct CasinoCard : ICard
    {
        private const byte FrenchSuitCount = 4;
        private const int CardNameValueOffset = 3;
        private const int FrenchSuitValueOffset = 0;

        private readonly static ECardName[] CardNameOrderedByValue = new ECardName[13]
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
            ECardName.Two
        };
        private readonly static EFrenchSuit[] FrenchSuitsOrderedByValue = new EFrenchSuit[4]
        {
            EFrenchSuit.Spade,
            EFrenchSuit.Club,
            EFrenchSuit.Diamond,
            EFrenchSuit.Heart
        };

        public byte CardId { get; private set; }
        public EFrenchSuit Suit { get; private set; }

        public ECardName CardName { get; private set; }

        public CasinoCard(EFrenchSuit inSuit, ECardName inValue)
        {
            this.Suit = inSuit;
            this.CardName = inValue;
            this.CardId = 0;
            this.CardId = CalculateCardId();
        }

        private CasinoCard(int cardValue, int suitValue)
        {
            this.Suit = SuitValueToEnum(suitValue);
            this.CardName = CardValueToEnum(cardValue);
            this.CardId = 0;
            this.CardId = CalculateCardId();
        }

        public bool Equals(CasinoCard other)
        {
            return other.CardName == this.CardName && other.Suit == this.Suit;
        }

        public override string ToString()
        {
#if UNITY_EDITOR
            if (CardName != ECardName.Two)
            {
                // return CardName >= ECardName.J ? $"{CardId}:{CardName}{GetSuitSymbol(Suit)}" : $"{CardId}:{(int)GetCardValue()}{GetSuitSymbol(Suit)}";
                return CardName >= ECardName.J ? $"{CardName}{GetSuitSymbol(Suit)}" : $"{(int)GetCardValue()}{GetSuitSymbol(Suit)}";
            }
            else
            {
                return $"{CardId}: 2{GetSuitSymbol(Suit)}";
            }
#else
			return $"{CardId}:{(int)GetCardValue()}{GetSuitSymbol(Suit)}";
#endif
        }

        public int GetCardValue()
        {
            if (CardName == ECardName.Unknow)
            {
                return -1;
            }

            for (int i = 0; i < CardNameOrderedByValue.Length; i++)
            {
                if (CardNameOrderedByValue[i] == CardName)
                {
                    return CardNameValueOffset + i;
                }
            }
            return -1;
        }

        public int GetSuitValue()
        {
            if (Suit == EFrenchSuit.Unknow)
            {
                return -1;
            }

            for (int i = 0; i < FrenchSuitsOrderedByValue.Length; i++)
            {
                if (FrenchSuitsOrderedByValue[i] == Suit)
                {
                    return FrenchSuitValueOffset + i;
                }
            }
            return -1;
        }

        private byte CalculateCardId()
        {
            int id = GetCardValue() * FrenchSuitCount + GetSuitValue();
            return (byte)id;
        }


        public static ECardName CardValueToEnum(int value)
        {
            int index = value - CardNameValueOffset;
            if (index >= 0 && index < CardNameOrderedByValue.Length)
            {
                return CardNameOrderedByValue[index];
            }
            return ECardName.Unknow;
        }

        public static EFrenchSuit SuitValueToEnum(int value)
        {
            int index = value - FrenchSuitValueOffset;
            if (index >= 0 && index < FrenchSuitsOrderedByValue.Length)
            {
                return FrenchSuitsOrderedByValue[index];
            }
            return EFrenchSuit.Unknow;
        }

        public int CompareTo(object obj)
        {
            if (obj is CasinoCard other)
            {
                int valueCompare = GetCardValue().CompareTo(other.GetCardValue());
                if (valueCompare != 0)
                {
                    return valueCompare;
                }
                // In case value is equal, compare the Suit
                return GetSuitValue().CompareTo(other.GetSuitValue());
            }
            return 0;
        }

        public static string GetSuitSymbol(EFrenchSuit suit)
        {
#if UNITY_EDITOR
            switch (suit)
            {
                case EFrenchSuit.Club:
                    return "♣";
                case EFrenchSuit.Diamond:
                    return "♦";
                case EFrenchSuit.Heart:
                    return "♥";
                case EFrenchSuit.Spade:
                    return "♠";
            }
#endif
            return $"{suit}";
        }

        public static string GetCardVisual(ECardName cardName)
        {
            switch (cardName)
            {
                case ECardName.Unknow:
                    return "";
                case ECardName.Three:
                    return "3";
                case ECardName.Four:
                    return "4";
                case ECardName.Five:
                    return "5";
                case ECardName.Six:
                    return "6";
                case ECardName.Seven:
                    return "7";
                case ECardName.Eight:
                    return "8";
                case ECardName.Nine:
                    return "9";
                case ECardName.Ten:
                    return "10";
                case ECardName.J:
                    return "J";
                case ECardName.Q:
                    return "Q";
                case ECardName.K:
                    return "K";
                case ECardName.A:
                    return "A";
                case ECardName.Two:
                    return "2";
            }
            return "";
        }

        public static ECardName NextCardInStraight(ECardName current)
        {
            if (current != ECardName.A && current != ECardName.Two)
            {
                return (ECardName)((int)current + 1);
            }

            return ECardName.Unknow;
        }

        public static CasinoCard FromCardId(byte cardId)
        {
            int cardValue = cardId / FrenchSuitCount;
            int suitValue = cardId % FrenchSuitCount;
            return new CasinoCard(cardValue, suitValue);
        }

        public static List<ICard> FromCardIdList(List<byte> idList)
        {
            List<ICard> cards = new List<ICard>();
            for (int i = 0; i < idList.Count; i++)
            {
                byte cardId = idList[i];
                CasinoCard card = CasinoCard.FromCardId(cardId);
                if (card.Suit != EFrenchSuit.Unknow && card.CardName != ECardName.Unknow)
                {
                    cards.Add(card);
                }
                else
                {
                    BELogger.LogE($"Card id {cardId} is wrong");
                }
            }
            return cards;
        }
    }

    public class CardDeck
    {
        private List<CasinoCard> _initCards = new List<CasinoCard>();
        private List<CasinoCard> _shuffledCards = new List<CasinoCard>();

        private CardDeck()
        {
            _initCards = GetInitCards();
            _shuffledCards = new List<CasinoCard>(_initCards);
            _shuffledCards.Sort();
        }

        private CardDeck(List<CasinoCard> predefine) : base()        
        {
            
            _shuffledCards = new List<CasinoCard>(predefine);
        }

        public CardDeck Shuffle()
        {
            _shuffledCards.Clear();
            _shuffledCards.AddRange(_initCards);
            _shuffledCards.Shuffle();
            return this;
        }

        public void ShuffleRemaining()
        {
            _shuffledCards.Shuffle();
        }

        public CasinoCard PullOutCard()
        {
            if (_shuffledCards.Count > 0)
            {
                CasinoCard card = _shuffledCards[0];
                _shuffledCards.RemoveAt(0);
                return card;
            }
            return default;
        }

        public List<CasinoCard> PullOutCard(int numberOfCard)
        {
            List<CasinoCard> outResult = new List<CasinoCard>();
            for (int i = 0; i < numberOfCard; i++)
            {
                outResult.Add(PullOutCard());
            }
            return outResult;
        }

        public CasinoCard PullOutCard(byte cardId)
        {
            int cardIdx = _shuffledCards.FindIndex((c) => c.CardId == cardId);
            if (cardIdx >= 0)
            {
                CasinoCard card = _shuffledCards[cardIdx];
                _shuffledCards.RemoveAt(cardIdx);
                return card;
            }
            return default;
        }
        
        public bool PutBack(CasinoCard card)
        {
            if (card.CardName != ECardName.Unknow && card.Suit != EFrenchSuit.Unknow)
            {
                return _shuffledCards.AddIfNotExist(card, (c) => c.CardId == card.CardId);
            }
            return false;
        }

        public List<CasinoCard> GetInitCards()
        {
            List<CasinoCard> initCards = new List<CasinoCard>();
            for (int i = (int)ECardName.Two; i <= (int)ECardName.A; i++)
            {
                initCards.Add(new CasinoCard(EFrenchSuit.Club, (ECardName)i));
                initCards.Add(new CasinoCard(EFrenchSuit.Diamond, (ECardName)i));
                initCards.Add(new CasinoCard(EFrenchSuit.Heart, (ECardName)i));
                initCards.Add(new CasinoCard(EFrenchSuit.Spade, (ECardName)i));
            }
            return initCards;
        }

        public IEnumerator<CasinoCard> GetRemaingCardEnumerator()
        {
            return _shuffledCards.GetEnumerator();
        }

        public int RemaingCount()
        {
            return _shuffledCards.Count;
        }

        public void Sort()
        {
            _shuffledCards.Sort();
        }

        public bool Copy(CardDeck other)
        {
            if (other == null)
            {
                return false;
            }

            _shuffledCards.Clear();
            IEnumerator<CasinoCard> it = other.GetRemaingCardEnumerator();
            while(it.MoveNext())
            {
                _shuffledCards.Add(it.Current);
            }

            return true;
        }

        public bool IsNewDeck()
        {
            return _shuffledCards.Count == _initCards.Count;
        }

        public static CardDeck NewDeck(bool shouldShuffle = false)
        {
            CardDeck deck = new CardDeck();
            if (shouldShuffle)
            {
                deck.Shuffle();
            }
            return deck;
        }

        public static CardDeck CreateDeckFrom(List<byte> cardIdList)
        {
            List<CasinoCard> allCard = new List<CasinoCard>();

            for (int i = 0; i < cardIdList.Count; i++)
            {
                byte id = cardIdList[i];
                if (allCard.FindIndex(c => c.CardId == id) < 0)
                {
                    CasinoCard card = CasinoCard.FromCardId(id);
                    if (card.CardName != ECardName.Unknow && card.Suit != EFrenchSuit.Unknow)
                    {
                        allCard.Add(card);
                    }
                }
            }

            return new CardDeck(allCard);
        }


        public void PrintAllCardId()
        {
            for (int i = 0; i < _initCards.Count; i++)
            {
                BELogger.LogI(_initCards[i]);
            }
        }
    }
}
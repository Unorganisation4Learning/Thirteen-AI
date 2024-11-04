namespace Game.Thirteen
{
    using Game.Casino;
    using Game.Core;
    using System.Collections.Generic;

    public enum ECombination
    {
        None = 0,
        Single = 1 << 0,
        Pair = 1 << 1,
        ThreeOfKind = 1 << 2,
        FourOfKind = 1 << 3,
        ThreePairs = 1 << 4,
        FourPairs = 1 << 5,
        Straight = 1 << 6
    }
    public interface ICloneable<T>
    {
        T Clone();
    }

    public interface ICopyable<T>
    {
        void Copy(T target);
    }

    /// <summary>
    /// Helper class to validate a combination
    /// </summary>
    public abstract class CombinationPattern : 
        ICloneable<CombinationPattern>, 
        ICopyable<CombinationPattern>
    {
        /// <summary>
        /// Should NOT trust CardId in case neither ECardName equal Unknow nor EFrenchSuit equal Unknow
        /// </summary>
        protected struct PatternCard
        {
            public CasinoCard Card;

            public PatternCard(byte cardId)
            {
                Card = CasinoCard.FromCardId(cardId);
            }

            public PatternCard(EFrenchSuit suit, ECardName name)
            {
                Card = new CasinoCard(suit, name);
            }

            /// <summary>
            //// If Card.CardName is unknow that mean any card name can be accepable             
            /// </summary>
            public bool IsName(ECardName name)
            {
                return Card.CardName == ECardName.Unknow || Card.CardName == name;
            }

            /// <summary>
            //// If Card.Suit is unknow that mean any card suit can be accepable             
            /// </summary>
            public bool IsSuit(EFrenchSuit suit)
            {
                return Card.Suit == EFrenchSuit.Unknow || Card.Suit == suit;
            }
        }

        public ECombination PatternName { get; private set; }

        protected List<PatternCard> Patterns { get; set; }

        public List<byte> FilledCardId { get; private set; }

        public CombinationPattern() { }

        protected CombinationPattern(ECombination patternName, List<byte> initCardId)
        {
            PatternName = patternName;
            FilledCardId = new List<byte>(initCardId);
            Patterns = null;
            Initalize();
        }

        protected CombinationPattern(ECombination patternName, CombinationPattern initPattern)
            : this(patternName, initPattern.FilledCardId)
        {

        }

        public abstract CombinationPattern Clone();

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is CombinationPattern other)
            {
                return
                    // might need to compare this.Patterns in the future
                    PatternName == other.PatternName &&
                    FilledCardId.CompareValue(other.FilledCardId);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public virtual void Copy(CombinationPattern other)
        {
            this.PatternName = other.PatternName;
            this.Patterns = new List<PatternCard>(other.Patterns);
            this.FilledCardId = new List<byte>(other.FilledCardId);
        }

        private void Initalize()
        {
            InitalizePattern();
            // CheckFilledCardId();
        }

        public virtual bool CheckFilledCardId()
        {
            if (Patterns.Count != FilledCardId.Count)
            {
                return false;
            }

            for (int i = 0; i < Patterns.Count; i++)
            {
                byte cardId = FilledCardId[i];
                CasinoCard card = CasinoCard.FromCardId(cardId);
                PatternCard patternCard = Patterns[i];
                if (!patternCard.IsName(card.CardName) || !patternCard.IsSuit(card.Suit))
                {
                    return false;
                }
            }
            return true;
        }

        public bool AppendCardId(byte cardId)
        {
            FilledCardId.Add(cardId);
            bool result = CheckFilledCardId();
            if (!result)
            {
                FilledCardId.RemoveAt(FilledCardId.Count - 1);
            }
            return result;
        }

        protected virtual void InitalizePattern()
        {
            Patterns = new List<PatternCard>();
            // Can be oveeride base on purpose
            for (int i = 0; i < FilledCardId.Count; i++)
            {
                Patterns.Add(new PatternCard(FilledCardId[i]));
            }
        }

        public virtual bool IsPatternFulill()
        {
            switch (PatternName)
            {
                case ECombination.Single:
                    return FilledCardId.Count == 1;
                case ECombination.Pair:
                    return FilledCardId.Count == 2;
                case ECombination.ThreeOfKind:
                    return FilledCardId.Count == 3;
                case ECombination.FourOfKind:
                    return FilledCardId.Count == 4;
                case ECombination.ThreePairs:
                    return FilledCardId.Count == 6;
                case ECombination.FourPairs:
                    return FilledCardId.Count == 8;
                case ECombination.Straight:
                    return FilledCardId.Count >= 3;
            }
            return false;
        }

        public bool Contains(ECardName cardName, EFrenchSuit suit)
        {
            for (int i = 0; i < FilledCardId.Count; i++)
            {
                byte id = FilledCardId[i];
                CasinoCard card = CasinoCard.FromCardId(id);
                if (cardName == card.CardName)
                {
                    if (suit == EFrenchSuit.Unknow)
                    {
                        return true;
                    }
                    else if (suit == card.Suit)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override string ToString()
        {
            string s = null;
            List<ICard> cards = CasinoCard.FromCardIdList(FilledCardId);
            s = string.Join(" ", cards);
            return s;
        }

        public static ECombination GetUpgradePatterns(ECombination current)
        {
            switch (current)
            {
                case ECombination.Single:
                    return ECombination.Pair | ECombination.Straight;
                case ECombination.Pair:
                    return ECombination.ThreeOfKind | ECombination.ThreePairs;
                case ECombination.ThreeOfKind:
                    return ECombination.FourOfKind;
                case ECombination.FourOfKind:
                    return ECombination.None;
                case ECombination.ThreePairs:
                    return ECombination.FourPairs;
                case ECombination.FourPairs:
                    return ECombination.None;
                case ECombination.Straight:
                    return ECombination.Straight;
            }
            return ECombination.None;
        }

        public static CombinationPattern CreateUpgradePattern(CombinationPattern basePattern, byte appendCardId)
        {
            CombinationPattern result = null;
            switch (basePattern.PatternName)
            {
                case ECombination.Single:
                    {
                        result = new PairPattern(basePattern as SinglePattern);
                        if (result.AppendCardId(appendCardId))
                        {
                            return result;
                        }
                        result = new StraightPattern(basePattern as SinglePattern);
                        if (result.AppendCardId(appendCardId))
                        {
                            return result;
                        }
                    }
                    break;
                case ECombination.Pair:
                    {
                        result = new ThreeOfKindPattern(basePattern as PairPattern);
                        if (result.AppendCardId(appendCardId))
                        {
                            return result;
                        }

                        result = new ThreePairsPattern(basePattern as PairPattern);
                        if (result.AppendCardId(appendCardId))
                        {
                            return result;
                        }
                    }
                    break;
                case ECombination.ThreeOfKind:
                    {
                        result = new FourOfKindPattern(basePattern as ThreeOfKindPattern);
                        if (result.AppendCardId(appendCardId))
                        {
                            return result;
                        }
                    }
                    break;
                case ECombination.FourOfKind:
                    break;
                case ECombination.ThreePairs:
                    {
                        result = new ThreePairsPattern(basePattern as ThreePairsPattern);
                        if (result.AppendCardId(appendCardId))
                        {
                            return result;
                        }

                        result = new FourPairsPattern(basePattern as ThreePairsPattern);
                        if (result.AppendCardId(appendCardId))
                        {
                            return result;
                        }
                    }
                    break;
                case ECombination.FourPairs:
                    {
                        result = new FourPairsPattern(basePattern as FourPairsPattern);
                        if (result.AppendCardId(appendCardId))
                        {
                            return result;
                        }
                    }
                    break;
                case ECombination.Straight:
                    result = new StraightPattern(basePattern as StraightPattern);
                    if (result.AppendCardId(appendCardId))
                    {
                        return result;
                    }
                    break;
            }
            return null;
        }
    }

    public class SinglePattern : CombinationPattern
    {
        private SinglePattern() : base() { }

        public SinglePattern(byte cardId) : base(ECombination.Single, new List<byte>() { cardId })
        {

        }

        public override CombinationPattern Clone()
        {
            SinglePattern p = new SinglePattern();
            p.Copy(this);
            return p;
        }
    }

    public class PairPattern : CombinationPattern
    {
        public PairPattern(SinglePattern single) : base(ECombination.Pair, single)
        {

        }
        protected PairPattern() : base() { }

        protected override void InitalizePattern()
        {
            base.InitalizePattern();

            // Need 1 more card has same value to be a pair
            PatternCard single = Patterns[0];
            Patterns.Add(new PatternCard(EFrenchSuit.Unknow, single.Card.CardName));
        }

        public override CombinationPattern Clone()
        {
            PairPattern p = new PairPattern();
            p.Copy(this);
            return p;
        }
    }

    public class ThreeOfKindPattern : CombinationPattern
    {
        public ThreeOfKindPattern(PairPattern pair) : base(ECombination.ThreeOfKind, pair)
        {

        }

        protected ThreeOfKindPattern() : base() { }

        protected override void InitalizePattern()
        {
            base.InitalizePattern();

            // Need 1 more card has same value to be a three of kind
            PatternCard single = Patterns[0];
            Patterns.Add(new PatternCard(EFrenchSuit.Unknow, single.Card.CardName));
        }

        public override CombinationPattern Clone()
        {
            ThreeOfKindPattern p = new ThreeOfKindPattern();
            p.Copy(this);
            return p;
        }
    }

    public class FourOfKindPattern : CombinationPattern
    {
        public FourOfKindPattern(ThreeOfKindPattern three) : base(ECombination.FourOfKind, three)
        {

        }

        protected FourOfKindPattern() : base() { }

        protected override void InitalizePattern()
        {
            base.InitalizePattern();

            // Need 1 more card has same value to be a three of kind
            PatternCard single = Patterns[0];
            Patterns.Add(new PatternCard(EFrenchSuit.Unknow, single.Card.CardName));
        }

        public override CombinationPattern Clone()
        {
            FourOfKindPattern p = new FourOfKindPattern();
            p.Copy(this);
            return p;
        }
    }

    public class StraightPattern : CombinationPattern
    {
        // To initalize a straight, this can make a straight that contains 2 card only
        public StraightPattern(SinglePattern single) : base(ECombination.Straight, single) { }
        // This can make a straight that longer the input straight 1 more card
        public StraightPattern(StraightPattern straight) : base(ECombination.Straight, straight) { }

        protected StraightPattern() : base() { }

        protected override void InitalizePattern()
        {
            base.InitalizePattern();
            PatternCard lastBiggestCard = Patterns[Patterns.Count - 1];
            ECardName nextCardName = CasinoCard.NextCardInStraight(lastBiggestCard.Card.CardName);
            if (nextCardName != ECardName.Unknow)
            {
                Patterns.Add(new PatternCard(EFrenchSuit.Unknow, nextCardName));
            }
        }

        public override CombinationPattern Clone()
        {
            StraightPattern p = new StraightPattern();
            p.Copy(this);
            return p;
        }
    }

    public class ThreePairsPattern : CombinationPattern
    {
        public ThreePairsPattern(PairPattern pair) : base(ECombination.ThreePairs, pair) { }
        public ThreePairsPattern(ThreePairsPattern pairs) : base(ECombination.ThreePairs, pairs) { }
        protected ThreePairsPattern() : base() { }

        public override bool CheckFilledCardId()
        {
            if (FilledCardId.Count > 6)
            {
                return false;
            }

            return base.CheckFilledCardId();
        }

        protected override void InitalizePattern()
        {
            base.InitalizePattern();
            PatternCard previousCard = Patterns[Patterns.Count / 2];
            ECardName nextCardName = CasinoCard.NextCardInStraight(previousCard.Card.CardName);
            if (nextCardName != ECardName.Unknow)
            {
                Patterns.Add(new PatternCard(EFrenchSuit.Unknow, nextCardName));
            }
        }

        public override CombinationPattern Clone()
        {
            ThreePairsPattern p = new ThreePairsPattern();
            p.Copy(this);
            return p;
        }
    }

    public class FourPairsPattern : CombinationPattern
    {
        public FourPairsPattern(ThreePairsPattern pair) : base(ECombination.FourPairs, pair) { }
        public FourPairsPattern(FourPairsPattern pairs) : base(ECombination.FourPairs, pairs) { }
        protected FourPairsPattern() : base() { }

        public override bool CheckFilledCardId()
        {
            if (FilledCardId.Count > 8)
            {
                return false;
            }

            return base.CheckFilledCardId();
        }

        protected override void InitalizePattern()
        {
            base.InitalizePattern();
            PatternCard previousCard = Patterns[Patterns.Count / 2];
            ECardName nextCardName = CasinoCard.NextCardInStraight(previousCard.Card.CardName);
            if (nextCardName != ECardName.Unknow)
            {
                Patterns.Add(new PatternCard(EFrenchSuit.Unknow, nextCardName));
            }
        }

        public override CombinationPattern Clone()
        {
            FourPairsPattern p = new FourPairsPattern();
            p.Copy(this);
            return p;
        }
    }
}

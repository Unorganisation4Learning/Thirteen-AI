
namespace Game.Thirteen
{
    using Game.Casino;
    using Game.Core;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Contains a combination and extra information
    /// </summary>
    public class CombinationBuilder
    {
        public CombinationPattern Pattern { get; private set; }

        public List<byte> DeletedCardId { get; private set; }

        public bool IsUnusable { get; private set; } = true;

        /// <summary>
        /// Purpose to see how strong this is
        /// </summary>
        public uint Strength { get; private set; }

        /// <summary>
        /// Use for caching prevent calling Build method multiple times
        /// </summary>
        private CardCombination _fulfillCombination = null;

        public CombinationBuilder(CombinationPattern pattern)
        {
            this.Pattern = pattern;
            this.IsUnusable = this.Pattern.IsPatternFulill();
            this.DeletedCardId = new List<byte>();
            this.Strength = CaculateStrength(Pattern);
        }

        public CombinationBuilder(CombinationBuilder other)
        {
            this.Pattern = other.Pattern.Clone();
            this.IsUnusable = other.IsUnusable;
            this.DeletedCardId = new List<byte>(other.DeletedCardId);
            this.Strength = other.Strength;
        }

        public void MarkForDeleteCard(byte cardId)
        {
            if (Pattern.FilledCardId.Exists(id => id == cardId) && !DeletedCardId.Exists(id => id == cardId))
            {
                IsUnusable = false;
                DeletedCardId.Add(cardId);
            }
        }

        public void UnmarkForDeleteCard(byte cardId)
        {
            int idx = DeletedCardId.FindIndex(id => id == cardId);
            if (idx >= 0)
            {
                DeletedCardId.RemoveAt(idx);
                IsUnusable = DeletedCardId.Count == 0 && this.Pattern.IsPatternFulill();
            }
        }

        public void UnmarkForDeleteAll()
        {
            DeletedCardId.Clear();
            IsUnusable = this.Pattern.IsPatternFulill();
        }

        public CardCombination Build()
        {
            if (!IsUnusable)
            {
                return null;
            }
            else if (_fulfillCombination == null)
            {
                _fulfillCombination = ThirteenCardManager.CreateCardCombination(CasinoCard.FromCardIdList(Pattern.FilledCardId));
            }
            return _fulfillCombination;
        }

        public override string ToString()
        {
#if UNITY_EDITOR
            return $"IsUnusable:{IsUnusable} Strength:{Strength}\nPattern:{Pattern}";
#else
            return null;
#endif
        }

        public uint CaculateStrength(CombinationPattern pattern)
        {
            // Note:
            // 3 Spade id = 12
            // 2 Heart id = 63

            uint unit = 20;
            uint stregth = 0;
            // Base value will be sum of card id because higher card id is higher card, Only correct if a pattern compare agaist the same pattern type
            for (int i = 0; i < pattern.FilledCardId.Count; i++)
            {
                stregth += pattern.FilledCardId[i];
            }

            switch (pattern.PatternName)
            {
                case ECombination.None:
                case ECombination.Single:
                case ECombination.Pair:
                case ECombination.Straight:
                    break;
                case ECombination.ThreeOfKind:
                    // Because ThreeOfKind a litte rare, so try to keep it by adding more score
                    stregth += unit * 1;
                    break;
                case ECombination.ThreePairs:
                    stregth += unit * 2;
                    break;
                case ECombination.FourOfKind:
                    stregth += unit * 3;
                    break;
                case ECombination.FourPairs:
                    stregth += unit * 5;
                    break;
            }
            return stregth;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is CombinationBuilder other)
            {
                return
                    this.IsUnusable == other.IsUnusable &&
                    this.DeletedCardId.CompareValue(other.DeletedCardId) &&
                    this.Pattern.Equals(other.Pattern);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

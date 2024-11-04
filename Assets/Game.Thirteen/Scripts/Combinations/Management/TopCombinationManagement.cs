
namespace Game.Thirteen
{
    using Game.Core;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public enum ECollection : byte
    {
        None = 0,
        Single = 1,
        Pair = 2,
        ThreeOfKind = 3,
        Straight = 4,
        Special = 5,        
    }

    public class CardCombinationCollection
    {
        public ECollection CollectionName = ECollection.None;

        public List<CombinationBuilder> Collection;

        public virtual bool IsEmpty() { return Collection.IsEmpty(); }

        public virtual bool IsHighest(CombinationBuilder builder) { return false; }

        public virtual bool Delete(CombinationBuilder builder) { return false; }

        public static ECollection GetCollectName(CombinationBuilder builder)
        {
            return ECollection.None;
        }
    }

    public class TopCombinationManagement
    {
        public List<CardCombinationManagement> CombinationManagements { get; private set; }
        public List<CardCombinationCollection> CombinationCollections { get; private set; }

        public TopCombinationManagement() { }

        public void Initalize(List<CardCombinationManagement> initalCombinationManagements)
        {
            CombinationManagements = initalCombinationManagements;
            
            // Single category

            // Pair category

            // Three of kind category

            // Straight
            // Straight 3,4 .. 12

            // Special
            // 2, 3 pairs, 4 of kind, 4 pairs
            // 22, 4 of kind, 4 pairs
        }
    
        public CardCombinationCollection GetCollection(ECollection name)
        {
            return CombinationCollections.FirstOrDefault(c => c.CollectionName == name);
        }
    
        public bool IsHighest(CombinationBuilder builder)
        {
            return GetCollection(CardCombinationCollection.GetCollectName(builder)).IsHighest(builder);
        }

        public bool Delete(CombinationBuilder builder)
        {
            return GetCollection(CardCombinationCollection.GetCollectName(builder)).Delete(builder);
        }
    }
}

namespace Game.Thirteen
{
    using System.Linq;
    using Game.Casino;
    using System.Collections.Generic;

    /// <summary>
    /// Responsibility for generating and managing all possible card combinations for a player's hand
    /// </summary>
    public class CardCombinationManagement : IIdentify
    {
        public static ThirteenCardCombinationComparer CombinationComparer = new ThirteenCardCombinationComparer();

        /// <summary>
        /// All possible card combinations at the moment
        /// </summary>
        public List<CombinationBuilder> Builders { get; private set; } = new List<CombinationBuilder>();
        /// <summary>
        /// Unique combination builders
        /// </summary>
        public List<CombinationBuilder> UniqueBuilders { get; private set; } = new List<CombinationBuilder>();

        /// <summary>
        /// All possible card combinations by name at the moment
        /// </summary>
        public Dictionary<ECombination, List<CombinationBuilder>> BuildersByName { get; private set; } = new Dictionary<ECombination, List<CombinationBuilder>>();

        /// <summary>
        /// Inital card id list
        /// </summary>
        public List<byte> CardIdList { get; private set; } = new List<byte>();

        public uint Strength 
        { 
            get
            {
                uint strength = 0;
                for (int i = 0; i < Builders.Count; i++)
                {
                    if (Builders[i].IsUnusable)
                    {
                        strength += Builders[i].Strength;
                    }
                }
                return strength;
            }
        }

        private ulong _id = 0;

        public CardCombinationManagement(ulong id = 0)
        {
            SetId(id);
        }

        public CardCombinationManagement(CardCombinationManagement other)
        {
            this.Builders = new List<CombinationBuilder>(other.Builders.Count);
            for (int i = 0; i < other.Builders.Count; i++)
            {
                this.Builders.Add(new CombinationBuilder(other.Builders[i]));
            }
            this.CardIdList = new List<byte>(other.CardIdList);
            this.BuildersByName = new Dictionary<ECombination, List<CombinationBuilder>>();

            UniqueBuilders = new List<CombinationBuilder>();
            for (int i = 0; i < other.UniqueBuilders.Count; i++)
            {
                this.UniqueBuilders.Add(new CombinationBuilder(other.UniqueBuilders[i]));
            }

            CategorizeBuilders();
        }

        public ulong GetId()
        {
            return _id;
        }
        
        public bool SetId(ulong id)
        {
            _id = id;
            return true;
        }

        /// <summary>
        /// CreateBuilders those can be used to create a card combination
        /// </summary>
        /// <param name="cardIdList">Must be sorted</param>
        /// <returns></returns>
        public void SetCardList(List<byte> cardIdList)
        {
            CardIdList = new List<byte>(cardIdList);
            CardIdList.Sort();

            Builders.Clear();
            UniqueBuilders.Clear();
            List<CombinationPattern> patterns = CreateBuilders(CardIdList);
            for (int i = 0; i < patterns.Count; i++)
            {
                CombinationBuilder builder = new CombinationBuilder(patterns[i]);
                Builders.Add(builder);
            }

            CategorizeBuilders();
            UniqueBuilders = CollectUniqueBuilders();
        }

        public byte SmallestCardId()
        {
            if (CardIdList.Count == 0)
            {
                return 0;
            }
            byte min = CardIdList[0];
            for (int i = 1; i < CardIdList.Count; i++)
            {
                min = System.Math.Min(min, CardIdList[i]);
            }
            return min;
        }

        public List<CombinationBuilder> OptimizedBuilders()
        {
            List<CombinationBuilder> result = new List<CombinationBuilder>();
            result.AddRange(FindOrphanCombinationBuilders());
            result.AddRange(UniqueBuilders.Where(b => b.Pattern.PatternName != ECombination.Single));
            return result;
        }

        public void GetPlayableBuilders(List<CombinationBuilder> outList)
        {
            outList.Clear();
            List<CombinationBuilder> builders = UniqueBuilders;
            for (int i = 0; i < builders.Count; i++)
            {
                CombinationBuilder b = builders[i];
                if (b.IsUnusable)
                {
                    outList.Add(b);
                }
            }
        }

        private void CategorizeBuilders()
        {
            foreach (KeyValuePair<ECombination, List<CombinationBuilder>> it in BuildersByName)
            {
                it.Value.Clear();
            }
            
            // Categorize builders
            for (int i = 0; i < Builders.Count; i++)
            {
                CombinationBuilder builder = Builders[i];
                ECombination patternName = builder.Pattern.PatternName;
                if (BuildersByName.TryGetValue(patternName, out List<CombinationBuilder> ls))
                {
                    ls.Add(builder);
                }
                else
                {
                    BuildersByName.Add(patternName, new List<CombinationBuilder>() { builder });
                }
            }
        }

        public List<CombinationBuilder> CollectUniqueBuilders()
        {
            return CollectUniqueBuilders(Builders);            
        }

        public void MarkForDeleteCard(byte cardId)
        {
            for (int i = 0; i < Builders.Count; i++)
            {
                Builders[i].MarkForDeleteCard(cardId);
            }
        }

        public void UnmarkForDeleteCard(byte cardId)
        {
            for (int i = 0; i < Builders.Count; i++)
            {
                Builders[i].UnmarkForDeleteCard(cardId);
            }
        }

        public void DeleteUnusableBuilders()
        {
            for (int i = 0; i < Builders.Count; i++)
            {
                if (!Builders[i].IsUnusable)
                {
                    CombinationBuilder deletedBuilder = Builders[i];
                    Builders.RemoveAt(i);
                    i--;

                    foreach (byte id in deletedBuilder.Pattern.FilledCardId)
                    {
                        CardIdList.Remove(id);
                    }
                }
            }
            CategorizeBuilders();
            UniqueBuilders = CollectUniqueBuilders();
        }

        public void MarkForDelete(CombinationBuilder builder)
        {
            if (builder == null)
            {
                return;
            }

            if (BuildersByName.TryGetValue(builder.Pattern.PatternName, out List<CombinationBuilder> builders) && builders.Exists(b => b.Equals(builder)))
            {
                for (int i = 0; i < builder.Pattern.FilledCardId.Count; i++)
                {
                    byte deleteCard = builder.Pattern.FilledCardId[i];
                    MarkForDeleteCard(deleteCard);
                }
            }
        }

        public void UnmarkForDelete(CombinationBuilder builder)
        {
            if (builder == null)
            {
                return;
            }

            if (BuildersByName.TryGetValue(builder.Pattern.PatternName, out List<CombinationBuilder> builders) && builders.Exists(b => b.Equals(builder)))
            {
                for (int i = 0; i < builder.Pattern.FilledCardId.Count; i++)
                {
                    byte deleteCard = builder.Pattern.FilledCardId[i];
                    UnmarkForDeleteCard(deleteCard);
                }
            }
        }

        public void UnmarkForDeleteAll()
        {
            List<CombinationBuilder> builders = Builders;
            for (int i = 0; i < builders.Count; i++)
            {
                CombinationBuilder b = builders[i];
                if (!b.IsUnusable)
                {
                    b.UnmarkForDeleteAll();
                }
            }
        }

        public List<CombinationBuilder> FindOrphanCombinationBuilders()
        {
            return FindOrphanCombinationBuildersFrom(UniqueBuilders);
        }

        public List<CombinationBuilder> FindCounterBuildersFor(CardCombination other, bool useSmartBuilders = true)
        {
            if (other == null)
            {
                return useSmartBuilders ? OptimizedBuilders() : UniqueBuilders;
            }

            List<CombinationBuilder> result = new List<CombinationBuilder>();
            List<CombinationBuilder> builders = useSmartBuilders ? OptimizedBuilders() : UniqueBuilders;
            for (int i = 0; i < builders.Count; i++)
            {
                CombinationBuilder b = builders[i];
                if (b.IsUnusable && CombinationComparer.Compare(other, b.Build()) == ECardCombinationComparisionResult.RightGreater)
                {
                    result.Add(b);
                }
            }
            return result;
        }

        public uint MinPlayable(int startIdxFromPlayable, uint lastMinValue)
        {
            uint counter = 0;
            bool isInital = true;

            List<CombinationBuilder> playableBuilders = new List<CombinationBuilder>();
            do
            {
                GetPlayableBuilders(playableBuilders);
                if (playableBuilders.Count > 0)
                {
                    counter++;
                    CombinationBuilder chosenBuilder = isInital ? playableBuilders[startIdxFromPlayable] : playableBuilders[0];
                    MarkForDelete(chosenBuilder);
                    isInital = false;
                }
                if (counter >= lastMinValue)
                {
                    UnmarkForDeleteAll();
                    return lastMinValue;
                }
            }
            while (playableBuilders.Count > 0);
            UnmarkForDeleteAll();
            return counter;
        }

        /// <summary>
        /// Create all possible combinations
        /// </summary>                
        public static List<CombinationPattern> CreateBuilders(List<byte> cardIdList)
        {
            List<CombinationPattern> builders = new List<CombinationPattern>();

            for (int i = 0; i < cardIdList.Count; i++)
            {
                AppendBuilders(builders, cardIdList[i]);
            }

            for (int i = 0; i < builders.Count; i++)
            {
                if (!builders[i].IsPatternFulill())
                {
                    builders.RemoveAt(i);
                    i--;
                }
            }

            return builders;
        }

        /// <summary>
        /// Find all orphan combinations
        /// Orphan combination is a single combination and that doesn't make any other combination
        /// </summary>
        public static List<CombinationBuilder> FindOrphanCombinationBuildersFrom(List<CombinationBuilder> collection)
        {
            List<CombinationBuilder> orhans = collection.Where(c => c.Pattern.PatternName == ECombination.Single).ToList();
            
            for (int i = 0; i < orhans.Count ; i++)
            {
                CombinationBuilder single = orhans[i];
                foreach (var builder in collection)
                {
                    if (builder.Pattern.PatternName == ECombination.Single)
                    {
                        continue;
                    }
                    if (builder.Pattern.FilledCardId.Exists((cardId) => cardId == single.Pattern.FilledCardId[0]))
                    {
                        orhans.RemoveAt(i);
                        i--;
                        break;
                    }
                }
            }
            return orhans;
        }

        private static void AppendBuilders(List<CombinationPattern> currentBuilders, byte cardId)
        {
            List<CombinationPattern> newBuilders = new List<CombinationPattern>
            {
                new SinglePattern(cardId),
            };

            for (int i = 0; i < currentBuilders.Count; i++)
            {
                CombinationPattern current = currentBuilders[i];
                CombinationPattern newBuilder = CombinationPattern.CreateUpgradePattern(current, cardId);
                if (newBuilder != null)
                {
                    newBuilders.Add(newBuilder);
                }
            }

            currentBuilders.AddRange(newBuilders);
        }

        /// <summary>
        /// Collect uniquie combination builders
        /// basically they are combination which don't have duplicated card
        /// </summary>
        private static List<CombinationBuilder> CollectUniqueBuilders(List<CombinationBuilder> originalList)
        {
            List<SimilarBuilder> similarCollection = new List<SimilarBuilder>();

            for (int i = 0; i < originalList.Count; i++)
            {
                bool isAdded = false;
                CombinationBuilder builder = originalList[i];

                // Try to add to an exists similar collection
                foreach (SimilarBuilder similar in similarCollection)
                {
                    if (similar.TryAppend(builder))
                    {
                        isAdded = true;
                        break;
                    }
                }

                if (!isAdded)
                {
                    // Create new similar collection
                    SimilarBuilder s = new SimilarBuilder(builder);
                    similarCollection.Add(s);
                }
            }
        
            List<CombinationBuilder> uniqueBuilders = new List<CombinationBuilder>();
            for (int i = 0; i < similarCollection.Count; i++)
            {
                uniqueBuilders.AddRange(similarCollection[i].CollectUniqueBuilders());
            }
            return uniqueBuilders;
        }
    }
}

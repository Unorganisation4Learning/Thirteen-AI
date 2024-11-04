namespace Game.Thirteen
{
    using Game.AI;
    using Game.Casino;
    using Game.Core;
    using Game.Core.Logger;
    using System.Collections.Generic;
    using System.Linq;

    public class ThirteenManualAI : IBEAgent
    {
        private const string Tag = "[" + nameof(ThirteenManualAI) + "]";

        public CardCombinationManagement RootManagement { get; private set; }

        public ManualPlayableNode MinPlayableTree { get; private set; }

        public ushort MinPlayable { get; private set; }

        public List<CardCombinationManagement> OtherCardManagements { get; private set; }

        public List<CombinationBuilder> HighestPlayables { get; private set; }

        public ThirteenManualAI(CardCombinationManagement management)
        {
            Initalize(management);
        }

        private void Initalize(CardCombinationManagement rootManagement)
        {
            this.RootManagement = rootManagement;
            this.MinPlayable = CalculateMinPlayableAfterPlay(RootManagement, null, out ManualPlayableNode minPlayableTree);
            this.MinPlayableTree = minPlayableTree;
            this.HighestPlayables = null;
        }

        public void SetOtherCardManagements(List<CardCombinationManagement> others)
        {
            this.OtherCardManagements = others;
            this.HighestPlayables = FindAllHighestPlayables(RootManagement);
        }

        public void Reset(CardCombinationManagement rootManagement, List<CardCombinationManagement> others)
        {
            Initalize(rootManagement);
            SetOtherCardManagements(others);
        }

        public List<CombinationBuilder> FindAllHighestPlayables(CardCombinationManagement mag)
        {
            List<CombinationBuilder> result = new List<CombinationBuilder>();
            List<CombinationBuilder> playables = new List<CombinationBuilder>();
            mag.GetPlayableBuilders(playables);
            for (int i = 0; i < playables.Count; i++)
            {
                CombinationBuilder builder = playables[i];
                if (IsHighest(builder))
                {
                    result.Add(builder);
                }
            }

            return result;
        }

        public List<CombinationBuilder> FindAllPlayables(CardCombinationManagement m, bool excludeHighest,
            bool excludeSpecial)
        {
            List<CombinationBuilder> playables = new List<CombinationBuilder>();
            m.GetPlayableBuilders(playables);

            if (!excludeHighest && !excludeSpecial)
            {
                return playables;
            }

            for (int i = 0; i < playables.Count; i++)
            {
                CombinationBuilder b = playables[i];
                if (IsHighest(b))
                {
                    playables.RemoveAt(i);
                    i--;
                }

                if (excludeSpecial)
                {
                    bool isSpecial = b.Pattern.PatternName == ECombination.FourOfKind ||
                                     b.Pattern.PatternName == ECombination.ThreePairs ||
                                     b.Pattern.PatternName == ECombination.FourPairs;
                    if (isSpecial)
                    {
                        playables.RemoveAt(i);
                        i--;
                    }
                }
            }

            return playables;
        }

        public bool IsHighest(CombinationBuilder builder)
        {
            if (!this.OtherCardManagements.IsEmpty())
            {
                for (int i = 0; i < this.OtherCardManagements.Count; i++)
                {
                    if (!this.OtherCardManagements[i].FindCounterBuildersFor(builder.Build()).IsEmpty())
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool CanFinish()
        {
            return RootManagement.UniqueBuilders.IsCountLessOrEqual(1);
        }

        private CardCombinationManagement GetOpponentHasLeastCard()
        {
            if (OtherCardManagements.Count == 1)
            {
                return OtherCardManagements[0];
            }

            CardCombinationManagement min = OtherCardManagements[0];
            for (int i = 1; i < OtherCardManagements.Count; i++)
            {
                CardCombinationManagement m = OtherCardManagements[i];
                if (m.CardIdList.Count < min.CardIdList.Count)
                {
                    min = m;
                }
            }

            return min;
        }

        public List<CombinationBuilder> FirstListCombinationOwnedHighest(bool excludeFirstHighest)
        {
            foreach (KeyValuePair<ECombination, List<CombinationBuilder>> item in RootManagement.BuildersByName)
            {
                if (excludeFirstHighest && item.Value.Count <= 1)
                {
                    continue;
                }

                List<CombinationBuilder> builders = item.Value;
                foreach (CombinationBuilder b in builders)
                {
                    if (IsHighest(b))
                    {
                        if (excludeFirstHighest)
                        {
                            return builders.Where(builder => builder != b).ToList();
                        }

                        return new List<CombinationBuilder>(builders);
                    }
                }
            }

            return null;
        }

        public CombinationBuilder FindAgainstCombination(CardCombination otherCombination, bool isFirstGame = false)
        {
            if (otherCombination == null)
            {
                // I'm go first
                BELogger.LogI($"{Tag} I'm go first");

                if (isFirstGame)
                {
                    List<CombinationBuilder> builders = new List<CombinationBuilder>();
                    byte smallestCard = RootManagement.SmallestCardId();
                    this.RootManagement.GetPlayableBuilders(builders);
                    for (int i = 0; i < builders.Count; i++)
                    {
                        if (builders[i].Pattern.FilledCardId.Contains(smallestCard))
                        {
                            BELogger.LogI($"{Tag} I'm going to play the smallest combination first");
                            return builders[i];
                        }
                    }
                }

                if (CanFinish())
                {
                    // Finish
                    BELogger.LogI($"{Tag} I play the last combination");
                    return this.RootManagement.UniqueBuilders[0];
                }

                if (this.RootManagement.UniqueBuilders.Count == 2)
                {
                    BELogger.LogI($"{Tag} I have 2 combination left");
                    int comCount = this.RootManagement.UniqueBuilders.Count;
                    // In case AI only 2 combination left
                    for (int i = 0; i < comCount; i++)
                    {
                        CombinationBuilder comBuilder = this.RootManagement.UniqueBuilders[i];
                        if (IsHighest(comBuilder))
                        {
                            // Play the highest first
                            BELogger.LogI($"{Tag} I will play the highest builder:{comBuilder}");
                            return comBuilder;
                        }
                    }

                    // Random because AI does not has highest playable
                    int randIdx = UnityEngine.Random.Range(0, comCount);
                    CombinationBuilder builder = this.RootManagement.UniqueBuilders[randIdx];
                    BELogger.LogI($"{Tag} I dont have any highest, will play random builder:{builder}");
                    return builder;
                }

                // In case AI has more than 2 playables
                BELogger.LogI($"{Tag} I have more than 2 combination");
                // 1. Find Combination which AI own the highest playable

                List<CombinationBuilder> playablesExcludedHighest = FirstListCombinationOwnedHighest(true);
                if (!playablesExcludedHighest.IsEmpty())
                {
                    int randIdx = UnityEngine.Random.Range(0, playablesExcludedHighest.Count);
                    CombinationBuilder builder = playablesExcludedHighest[randIdx];
                    BELogger.LogI($"{Tag} I will play random a combination which I own the highest type builder:{builder}");
                    return builder;
                }

                BELogger.LogI($"{Tag} I dont own any combination which I own the highest type");
                BELogger.LogI($"{Tag} I will try to prevent the opponent has least card finish the match");
                // 2. Find Combination which has highest playable not in hand of player has least card
                CardCombinationManagement hasLeastCardOppenent = GetOpponentHasLeastCard();
                List<CombinationBuilder> opPlayables = new List<CombinationBuilder>();
                hasLeastCardOppenent.GetPlayableBuilders(opPlayables);
                List<CombinationBuilder> highestBuilders = FindAllHighestPlayables(hasLeastCardOppenent);
                List<CombinationBuilder> playables = null;
                if (highestBuilders.IsEmpty())
                {
                    BELogger.LogI($"{Tag} The least card oppnent does have any highest combination");
                    BELogger.LogI($"{Tag} I will play any");
                    playables = FindAllPlayables(RootManagement, true, true);
                }
                else
                {
                    playables = FindAllPlayables(RootManagement, false, true);
                    BELogger.LogI($"{Tag} The least card oppnent own a highest combination of a type");
                    BELogger.LogI($"{Tag} I will avoid to play that type");
                    for (int i = 0; i < playables.Count; i++)
                    {
                        CombinationBuilder mine = playables[i];
                        bool isPlayable = highestBuilders.Find(hb =>
                            CardCombinationManagement.CombinationComparer.Compare(hb.Build(), mine.Build()) ==
                            ECardCombinationComparisionResult.LeftGreater) != null;
                        if (!isPlayable)
                        {
                            playables.RemoveAt(i);
                            i--;
                        }
                    }
                }

                if (!playables.IsEmpty())
                {
                    int randIdx = UnityEngine.Random.Range(0, playables.Count);
                    CombinationBuilder builder = playables[randIdx];
                    BELogger.LogI($"{Tag} I play builder:{builder}");
                    return builder;
                }

                // 3. Find Combiniation which has highest playable owned by player stands on left side of AI
            }
            else
            {
                BELogger.LogI($"{Tag} I play after another");
                // Have to against the other
                List<CombinationBuilder> playables = this.RootManagement.FindCounterBuildersFor(otherCombination, true);
                CombinationBuilder highestPlayable = playables.Find(p => IsHighest(p));

                if (playables.Count == 0)
                {
                    // Cannot be able to move
                    BELogger.LogI($"{Tag} I dont have any playable, skip");
                    return null;
                }

                // Has highest playable
                if (highestPlayable != null)
                {
                    BELogger.LogI($"{Tag} I have a highest playable highestPlayable:{highestPlayable}");
                    // Has less than or equal 2 playable and AI has the highest, play the highest first
                    if (this.RootManagement.UniqueBuilders.Count <= 2)
                    {
                        BELogger.LogI($"{Tag} I have less than or equal 2 combination, I will play the highes first");
                        return highestPlayable;
                    }

                    // Has more that 2 playable
                    if (this.RootManagement.UniqueBuilders.Count > 2)
                    {
                        BELogger.LogI($"{Tag} I have more than 2 combination");
                        for (int i = 0; i < playables.Count; i++)
                        {
                            CombinationBuilder builder = playables[i];
                            if (builder != highestPlayable)
                            {
                                BELogger.LogI($"{Tag} I will play the first combination which is not the highest I own buider:{builder}");
                                // Play the first playable that is not the highest AI own
                                return builder;
                            }
                        }
                    }
                }

                // Does has highest playable
                if (highestPlayable == null && !playables.IsEmpty())
                {
                    BELogger.LogI($"{Tag} I dont have a highest combination, I will play randomly");
                    // Random rate 50/50 for play and skip
                    int randThreshold = playables.Count * 2;
                    int randIdx = UnityEngine.Random.Range(0, randThreshold);
                    BELogger.LogI($"{Tag} random values randIdx:{randIdx} randThreshold:{randThreshold}");

                    if (randIdx < playables.Count)
                    {
                        CombinationBuilder builder = playables[randIdx];
                        BELogger.LogI($"{Tag} I play builder:{builder}");
                        return builder;
                    }
                }
            }

            BELogger.LogI($"{Tag} I skip the turn");
            // Skip or cannot be able to move
            return null;
        }

        public static ushort CalculateMinPlayableAfterPlay(CardCombinationManagement management,
            CombinationBuilder builder, out ManualPlayableNode minPlayableTree)
        {
            ManualPlayableNode.MinPlayableFound = ushort.MaxValue;
            CardCombinationManagement mag = new CardCombinationManagement(management);
            mag.MarkForDelete(builder);
            mag.DeleteUnusableBuilders();
            minPlayableTree = new ManualPlayableNode(0, mag, null);
            minPlayableTree.BuildTree();
            return ManualPlayableNode.MinPlayableFound;
        }
    }
}
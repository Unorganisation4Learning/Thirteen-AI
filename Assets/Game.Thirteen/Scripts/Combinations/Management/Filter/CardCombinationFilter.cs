using Game.Casino;
using Game.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Thirteen
{
    public abstract class CardCombinationFilter : IFilter<CombinationBuilder>
    {
        // Should be readonly
        public List<CombinationBuilder> Data { get; private set; }

        public List<CombinationBuilder> Results { get; private set; }

        public CardCombinationFilter()
        {
            SetInput(null);
            Results = new List<CombinationBuilder>();
        }

        public CardCombinationFilter(List<CombinationBuilder> data) : this()
        {
            SetInput(data);
        }

        public void SetInput(List<CombinationBuilder> data)
        {
            Data = data;
        }

        public abstract List<CombinationBuilder> RunFilter();
    }

    public class CombinationNameFilter : CardCombinationFilter
    {
        public ECombination FilterName = ECombination.Single;

        public override List<CombinationBuilder> RunFilter()
        {
            Results.Clear();
            if (!Data.IsEmpty())
            {
                for (int i = 0; i < Data.Count; i++)
                {
                    CombinationBuilder builder = Data[i];
                    if (builder.Pattern.PatternName == FilterName)
                    {
                        Results.Add(builder);
                    }
                }
            }
            return Results;
        }
    }

    public class CombinationContainsCardFilter : CardCombinationFilter
    {
        /// <summary>
        /// Must be not unknow
        /// </summary>
        public ECardName Name = ECardName.Unknow;
        /// <summary>
        /// Unkow to accept any
        /// </summary>
        public EFrenchSuit Suit = EFrenchSuit.Unknow;

        public override List<CombinationBuilder> RunFilter()
        {
            Results.Clear();
            if (Name != ECardName.Unknow && !Data.IsEmpty())
            {
                for (int i = 0; i < Data.Count; i++)
                {
                    CombinationBuilder builder = Data[i];
                    if (builder.Pattern.Contains(Name, Suit))
                    {
                        Results.Add(builder);
                    }
                }
            }
            return Results;
        }
    }
}

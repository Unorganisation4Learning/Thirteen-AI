
namespace Game.Thirteen
{
    using Game.Core;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Game.Casino;
    public class MinMaxTree
    {
        public CardCombinationManagement Minimizer { get; private set; }
        public CardCombinationManagement Maximizer { get; private set; }
        public MMNode Root { get; private set; }

        public MinMaxTree(CardCombinationManagement minimizer, CardCombinationManagement maximizer)
        {
            Minimizer = minimizer;
            Maximizer = maximizer;
        }

        public MMNode Build()
        {
            Root = BuildTreeFrom(Minimizer, Maximizer, true, null);
            return Root;
        }

        public static MMNode BuildTreeFrom(
            CardCombinationManagement minimizer, 
            CardCombinationManagement maximizer,
            bool isMaximizer,
            CardCombination initDecision)
        {
            MMNode root = new MMNode(null, initDecision, minimizer, maximizer, isMaximizer);
            root.CreateChildren();
            root.ScoreCalulate();
            return root;
        }
    }

    public class MMNode : INodeInformation
    {
        public MMNode Parent;
        public int Score;
        public bool IsMaximizer;
        public CardCombinationManagement Minimizer;
        public CardCombinationManagement Maximizer;
        public CardCombination Decision;
        public List<MMNode> Children;
        public MMNode Result;
        public bool IsRoot => Parent == null;

        public MMNode(MMNode parent, CardCombination decision, CardCombinationManagement minimizer, CardCombinationManagement maximizer, bool isMaximizer)
        {
            Minimizer = minimizer;
            Maximizer = maximizer;
            IsMaximizer = isMaximizer;
            Parent = parent;
            Children = null;
            Decision = decision;
        }

        public void CreateChildren()
        {
            // Take all possible moves of opponent
            CardCombinationManagement mag = new CardCombinationManagement(IsMaximizer ? Minimizer : Maximizer);

            List<CombinationBuilder> playableBuilders = mag.FindCounterBuildersFor(this.Decision, true);

            bool hasChild = playableBuilders.Count > 0;

            if (hasChild)
            {
                while (playableBuilders.Count > 0)
                {
                    CombinationBuilder decision = playableBuilders[0];
                    playableBuilders.RemoveAt(0);
                    CardCombinationManagement m = new CardCombinationManagement(mag);
                    m.MarkForDelete(decision);
                    m.DeleteUnusableBuilders();

                    MMNode node = null;
                    if (IsMaximizer)
                    {
                        node = new MMNode(this, decision.Build(), m, Maximizer, !IsMaximizer);
                    }
                    else
                    {
                        node = new MMNode(this, decision.Build(), Minimizer, m, !IsMaximizer);
                    }
                    if (Children == null)
                    {
                        Children = new List<MMNode>();
                    }
                    Children.Add(node);
                }

                for (int i = 0; i < Children.Count; i++)
                {
                    Children[i].CreateChildren();
                }
            }
        }

        public int ScoreCalulate()
        {
            Score = IsMaximizer ? int.MinValue : int.MaxValue;
            if (Children.IsEmpty())
            {
                // Calculate node owner's score
                CardCombinationManagement mag = IsMaximizer ? Maximizer : Minimizer;
                if (mag.Builders.Count > 0)
                {
                    Score = (int)mag.Strength;
                }
            }
            else
            {
                Children = Children.OrderBy(c => c.ScoreCalulate()).ToList();
                Result = IsMaximizer ? Children[Children.Count - 1] : Children[0];
                Score = Result.Score;
            }
            return Score;
        }
    }
}

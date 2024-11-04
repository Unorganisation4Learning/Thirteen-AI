namespace Game.Thirteen
{
    using Game.Core;
    using Game.Core.Logger;
    using System.Collections.Generic;
    using System.Linq;

    public class ManualPlayableNode
    {
        /// <summary>
        /// Only used for calculation, cache this after each caculate if you want to re-use the result later
        /// </summary>
        private static ushort MinPlayable = ushort.MaxValue;
        /// <summary>
        /// Only used for calculation, cache this after each caculate if you want to re-use the result later
        /// </summary>
        public static ushort MinPlayableFound = ushort.MaxValue;

        public ushort Depth { get; private set; }
        public List<ManualPlayableNode> Children { get; private set; }
        public CardCombinationManagement Management { get; private set; }
        public CombinationBuilder Chosen { get; private set; }
        public List<CombinationBuilder> PlayableBuilders { get; private set; }

        public ManualPlayableNode(ushort depth, CardCombinationManagement mag, CombinationBuilder chosen)
        {
            this.Depth = depth;
            this.Management = mag;
            this.Chosen = chosen;
            if (this.Chosen != null)
            {
                this.PlayableBuilders = new List<CombinationBuilder>();
                this.Management.GetPlayableBuilders(this.PlayableBuilders);
            }
        }

        public bool IsPossibleFoundLessPlayable()
        {
            return this.PlayableBuilders.Count <= MinPlayable;
        }

        private void ClearPlayable()
        {
            if (PlayableBuilders != null)
            {
                PlayableBuilders.Clear();
                PlayableBuilders = null;
            }
        }

        public void BuildTree()
        {
            List<CombinationBuilder> playableBuilders = new List<CombinationBuilder>();
            Management.GetPlayableBuilders(playableBuilders);

            for (int i = 0; i < playableBuilders.Count; i++)
            {
                CombinationBuilder chosenBuilder = playableBuilders[i];
                CardCombinationManagement cloneMag = new CardCombinationManagement(Management);
                cloneMag.MarkForDelete(chosenBuilder);
                cloneMag.DeleteUnusableBuilders();
                ManualPlayableNode child = new ManualPlayableNode((ushort)(Depth + 1), cloneMag, chosenBuilder);
                if (Children == null)
                {
                    Children = new List<ManualPlayableNode>();
                }
                Children.Add(child);
            }

            if (!Children.IsEmpty())
            {
                List<ManualPlayableNode> playables = new List<ManualPlayableNode>();
                MinPlayable = System.Math.Min(MinPlayable, (ushort)Children.Min(p => p.PlayableBuilders.SafeCount()));

                for (int i = 0; i < Children.Count; i++)
                {
                    ManualPlayableNode playable = Children[i];
                    if (playable.IsPossibleFoundLessPlayable())
                    {
                        playables.Add(playable);
                    }
                    else
                    {
                        playable.ClearPlayable();
                    }
                }
                if (playables.Count > 0)
                {
                    for (int i = 0; i < playables.Count; i++)
                    {
                        playables[i].BuildTree();
                    }
                }
            }
            else
            {
                MinPlayableFound = System.Math.Min(MinPlayableFound, this.Depth);
            }
        }
    }
}

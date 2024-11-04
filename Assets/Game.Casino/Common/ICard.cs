namespace Game.Casino
{
    public enum EFrenchSuit
    {
        Unknow      = 0,
        Club        = 1 << 0,
        Diamond     = 1 << 1,
        Heart       = 1 << 2,
        Spade       = 1 << 3,
    }

    public enum ECardName
    {
        Unknow = 0,
        Two = 1,
        Three = 2,
        Four = 3,
        Five = 4,
        Six = 5,
        Seven = 6,
        Eight = 7,
        Nine = 8,
        Ten = 9,
        J = 10,
        Q = 11,
        K = 12,
        A = 13,
    }

    public interface ICard : System.IComparable
    {
        /// <summary>
        /// Minimize data size, be careful byte.Max = [0;255]
        /// so only 256 card can be Id, if you need more than 256 card this field should be revised
        /// </summary>
        public byte CardId { get; }
        public EFrenchSuit Suit { get; }
        public ECardName CardName { get; }

        /// <summary>
        /// Used for card comparation
        /// </summary>
        /// <returns>Higher is high card value</returns>
        public int GetCardValue();

        /// <summary>
        /// The second condition in card 2 card have same value (GetCardValue), used for card comparation
        /// </summary>
        /// <returns>Higher is high card value</returns>
        public int GetSuitValue();
    }
}

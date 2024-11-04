namespace Game.Casino
{
    public enum ECardCombinationComparisionResult
	{
        CannotCompare = -999,
        RightGreater = -1,
        Equal = 0,
        LeftGreater = 1,
	}

    public interface ICardCombinationComparer
    {
        public ECardCombinationComparisionResult Compare(ICardCombination left, ICardCombination right, ICardCombinationComparableValidator validator);
    }
}

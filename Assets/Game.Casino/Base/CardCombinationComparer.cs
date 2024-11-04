namespace Game.Casino
{
    public class CardCombinationComparer : ICardCombinationComparer
    {
		public ICardCombinationComparableValidator ComparableValidator { get; protected set; }

		public ECardCombinationComparisionResult Compare(ICardCombination left, ICardCombination right, ICardCombinationComparableValidator overrideValidator)
		{
			if (overrideValidator != null)
			{
				ComparableValidator = overrideValidator;
			}
			
			if (ComparableValidator == null)
            {
				return (ECardCombinationComparisionResult)left.CompareTo(right);
            }
			else
			{
				if (ComparableValidator.IsComparable(left, right))
				{
					return (ECardCombinationComparisionResult)left.CompareTo(right);
				}
			}
			return ECardCombinationComparisionResult.CannotCompare;
		}
	}
}

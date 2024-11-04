namespace Game.Casino
{
	public interface ICardCombinationComparableValidator
	{
		public bool IsComparable(ICardCombination a, ICardCombination b);
	}
}
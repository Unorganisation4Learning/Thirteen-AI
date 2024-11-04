namespace Game.Casino
{
	public interface ICardCombination : System.IComparable<ICardCombination>
	{
		public System.Collections.Generic.List<ICard> OwnerCards { get; }
	}
}
using Game.Casino;
using System.Collections.Generic;
using System.Linq;

namespace Game.Thirteen
{
    public sealed class ThirteenCardCombinationComparer
    {
        public const string CombinationNameHighCard = "HighCard";
        public const string CombinationNamePair = "Pair";
        public const string CombinationNameThreeOfKind = "ThreeOfKind";
        public const string CombinationNameFourOfKind = "FourOfKind";
        public const string CombinationNameStraight = "Straight";
        public const string CombinationNameContinuoslyPairs = "ContinuoslyPairs";

        private CardCombinationComparer _cardCombinationComparer = new CardCombinationComparer();
        private Dictionary<string, ICardCombinationComparableValidator> _validators = new Dictionary<string, ICardCombinationComparableValidator>();

        public ECardCombinationComparisionResult Compare(ICardCombination left, ICardCombination right)
        {
            if (left == null || right == null)
            {
                return ECardCombinationComparisionResult.CannotCompare;
            }

            ICardCombinationComparableValidator validator;
            string combinationName = (left as CardCombination)?.CombinationName;
            string validatorKey = GetValidatorKey(left, right);
            if (_validators.ContainsKey(validatorKey))
            {
                validator = _validators[validatorKey];
            }
            else
            {
                validator = CreateCardCombinationComparableValidator(combinationName, left, right);
                _validators.Add(validatorKey, validator);
            }

            if (_cardCombinationComparer == default || _cardCombinationComparer == null)
            {
                return ECardCombinationComparisionResult.CannotCompare;
            }

            return _cardCombinationComparer.Compare(left, right, validator) ;
        }

        private string GetValidatorKey(ICardCombination left, ICardCombination right)
        {
            string c1 = (left as CardCombination)?.CombinationName;
            string c2 = (right as CardCombination)?.CombinationName;

            if (left.OwnerCards.All(card => card.CardName == ECardName.Two))
            {
                c1 += $"Two";
            }

            if (right.OwnerCards.All(card => card.CardName == ECardName.Two))
            {
                c2 += $"Two";
            }

            if (c1 != null && !c1.Equals(c2))
            {
                c1 += $"_{c2}";
            }

            return c1;
        }

        private ICardCombinationComparableValidator CreateCardCombinationComparableValidator(string combinationName,
            ICardCombination left, ICardCombination right)
        {
            switch (combinationName)
            {
                case CombinationNameHighCard:
                    {
                        SingleTwoCard_ContinuoslyPairsCardComparableValidator singleTwoCard_ContinuoslyPairsCard
                            = new SingleTwoCard_ContinuoslyPairsCardComparableValidator();
                        if(singleTwoCard_ContinuoslyPairsCard.IsComparable(left, right))
                        {
                            return singleTwoCard_ContinuoslyPairsCard;
                        }

                        SingleTwoCard_FourOfKindCardComparableValidator singleTwoCard_FourOfKindCard
                            = new SingleTwoCard_FourOfKindCardComparableValidator();
                        if(singleTwoCard_FourOfKindCard.IsComparable(left, right))
                        {
                            return singleTwoCard_FourOfKindCard;
                        }

                        return new NumberOfKindComparableValidator();
                    }
                case CombinationNamePair:
                    {
                        PairTwoCard_ContinuoslyPairsCardComparableValidator pairTwoCard_ContinuoslyPairsCard
                            = new PairTwoCard_ContinuoslyPairsCardComparableValidator();
                        if(pairTwoCard_ContinuoslyPairsCard.IsComparable(left, right))
                        {
                            return pairTwoCard_ContinuoslyPairsCard;
                        }

                        PairTwoCard_FourOfKindCardComparableValidator pairTwoCard_FourOfKindCard
                            = new PairTwoCard_FourOfKindCardComparableValidator();
                        if(pairTwoCard_FourOfKindCard.IsComparable(left, right))
                        {
                            return pairTwoCard_FourOfKindCard;
                        }

                        return new NumberOfKindComparableValidator();
                    }
                case CombinationNameThreeOfKind:
                case CombinationNameFourOfKind:
                    {
                        FourOfKindCard_ContinuoslyPairsCardComparableValidator fourOfKindCard_ContinuoslyPairsCard =
                            new FourOfKindCard_ContinuoslyPairsCardComparableValidator();
                        if (fourOfKindCard_ContinuoslyPairsCard.IsComparable(left, right))
                        {
                            return fourOfKindCard_ContinuoslyPairsCard;
                        }

                        FourOfKindCard_PairTwoCardComparableValidator fourOfKindCard_PairTwo
                            = new FourOfKindCard_PairTwoCardComparableValidator();
                        if (fourOfKindCard_PairTwo.IsComparable(left, right))
                        {
                            return fourOfKindCard_PairTwo;
                        }

                        FourOfKindCard_SingleTwoCardComparableValidator fourOfKindCard_SingleTwo =
                            new FourOfKindCard_SingleTwoCardComparableValidator();
                        if (fourOfKindCard_SingleTwo.IsComparable(left, right))
                        {
                            return fourOfKindCard_SingleTwo;
                        }

                        return new NumberOfKindComparableValidator();
                    }
                case CombinationNameStraight:
                    return new StraightComparableValidator();
                case CombinationNameContinuoslyPairs:
                    {
                        ContinuoslyPairsCard_FourOfKindCardComparableValidator continuoslyPairsCard_FourOfKind =
                            new ContinuoslyPairsCard_FourOfKindCardComparableValidator();
                        if (continuoslyPairsCard_FourOfKind.IsComparable(left, right))
                        {
                            return continuoslyPairsCard_FourOfKind;
                        }

                        ContinuoslyPairsCard_PairTwoCardComparableValidator continuoslyPairsCard_PairTwo =
                            new ContinuoslyPairsCard_PairTwoCardComparableValidator();
                        if (continuoslyPairsCard_PairTwo.IsComparable(left, right))
                        {
                            return continuoslyPairsCard_PairTwo;
                        }

                        ContinuoslyPairsCard_SingleTwoCardComparableValidator continuoslyPairsCard_SingleTwo =
                            new ContinuoslyPairsCard_SingleTwoCardComparableValidator();
                        if (continuoslyPairsCard_SingleTwo.IsComparable(left, right))
                        {
                            return continuoslyPairsCard_SingleTwo;
                        }

                        return new ContinuoslyPairsCardComparableValidator();
                    }
                    // TODO create new CardComparableValidator
            }
            throw new System.Exception($"CardCombinationComparableValidator of {combinationName} is NOT implemented yet");
        }
    }
}

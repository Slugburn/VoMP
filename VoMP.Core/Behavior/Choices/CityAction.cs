using System;
using VoMP.Core.Actions;
using VoMP.Core.CityCards;

namespace VoMP.Core.Behavior.Choices
{
    public class CityAction : ISpaceActionChoice
    {
        private readonly Player _player;
        private readonly LargeCityAction _space;

        public CityAction(Player player, LargeCityAction space)
        {
            _player = player;
            _space = space;
        }

        public ICityCard Card => _space.Card;

        public int OptimumValue => _space.Card.OptimumValue(_player);
        public Die Die { get; set; }
        public int Value { get; set; }

        public void Execute()
        {
            if (Die==null)
                throw new InvalidOperationException("Die has not been set.");
            if (Value==0)
                throw new InvalidOperationException("Value has not been set");
            _player.PlayDice(new [] {Die}, _space);
            var cost = GetCost();
            _player.PayCost(cost, "----------------------->" + _space.ToString());
            _player.GainReward(GetReward(), _space.ToString());
            _player.HasTakenActionThisTurn = true;
        }

        public bool IsValid()
        {
            return !_player.HasTakenActionThisTurn &&  _space.DiceCount == 0;
        }

        public bool CanGenerate(ResourceType resourceType)
        {
            return _space.Card.CanGenerate(_player, resourceType);
        }

        public Reward GetOptimumReward()
        {
            return _space.Card.GetReward(_player, OptimumValue);
        }

        public Cost GetCost()
        {
            return _space.Card.GetCost(Die.Value);
        }

        public Reward GetReward()
        {
            return _space.Card.GetReward(_player, Value);
        }
    }
}
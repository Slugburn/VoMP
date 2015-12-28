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

        public ICityCard Card { get; set; }

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

        public Cost GetCost()
        {
            return Card.GetCost(Die.Value);
        }

        public Reward GetReward()
        {
            return Card.GetReward(_player, Value);
        }
    }
}
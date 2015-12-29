using System;
using System.Collections.Generic;
using VoMP.Core.Actions;
using VoMP.Core.CityCards;

namespace VoMP.Core.Behavior.Choices
{
    public class CityAction : ISpaceActionChoice
    {
        private readonly Player _player;
        public ActionSpace Space { get; }

        public CityAction(Player player, LargeCityAction space)
        {
            _player = player;
            Space = space;
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
            _player.PlayDice(new [] {Die}, Space);
            var cost = GetCost();
            _player.PayCost(cost, "----------------------->" + Space.ToString());
            _player.GainReward(GetReward(), Space.ToString());
            _player.HasTakenActionThisTurn = true;
        }

        public bool IsValid()
        {
            return !_player.HasTakenActionThisTurn &&  Space.DiceCount == 0;
        }

        public Cost GetCost()
        {
            return Card.GetCost(Value);
        }

        public Reward GetReward()
        {
            return Card.GetReward(_player, Value);
        }

        public IList<Die> Dice => new[] {Die};
    }
}
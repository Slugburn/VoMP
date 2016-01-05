using System;
using System.Collections.Generic;
using VoMP.Core.Actions;
using VoMP.Core.CityCards;

namespace VoMP.Core.Behavior.Choices
{
    public class CityAction : ISpaceActionChoice
    {
        private readonly Player _player;

        public CityAction(Player player, LargeCityAction space)
        {
            _player = player;
            Space = space;
        }

        public ICityCard Card { get; set; }

        public Die Die { get; set; }
        public ActionSpace Space { get; }
        public int Value { get; set; }

        public void Execute()
        {
            if (Die == null)
                throw new InvalidOperationException("Die has not been set.");
            if (Value == 0)
                throw new InvalidOperationException("Value has not been set");
            _player.PlayDice(new[] {Die}, Space);
            var cost = Cost;
            _player.PayCost(cost, "----------------------->" + Space);
            _player.GainReward(Reward, Space.ToString());
            _player.HasTakenActionThisTurn = true;
        }

        public bool IsValid()
        {
            return !_player.HasTakenActionThisTurn && Space.DiceCount == 0;
        }

        public Cost Cost => Card.GetCost(Value);

        public Reward Reward => Card.GetReward(_player, Value);

        public IList<Die> Dice => new[] {Die};

        public override string ToString()
        {
            return $"{Cost}->{Reward}";
        }
    }
}
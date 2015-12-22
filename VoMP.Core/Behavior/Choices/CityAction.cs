using System.Linq;
using VoMP.Core.Actions;

namespace VoMP.Core.Behavior.Choices
{
    public class CityAction : IActionChoice
    {
        private readonly Player _player;
        private readonly LargeCityAction _space;

        public CityAction(Player player, LargeCityAction space)
        {
            _player = player;
            _space = space;
        }

        public int OptimumValue => _space.Card.OptimumValue(_player);
        public Die Die { get; set; }

        public void Execute()
        {
            _player.PlayDice(new [] {Die}, _space);
            var cost = GetCost();
            _player.PayCost(cost, "----------------------->" + _space.ToString());
            _player.GainReward(_space.Card.GetReward(_player, Die.Value), _space.ToString());
            _player.HasTakenActionThisTurn = true;
        }

        public bool IsValid()
        {
            return _player.CanPlayInActionSpace(_space) && !_space.Dice.Any();
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
    }
}
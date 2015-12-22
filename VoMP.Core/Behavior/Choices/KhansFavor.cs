using System.Linq;
using VoMP.Core.Actions;
using static VoMP.Core.ResourceType;

namespace VoMP.Core.Behavior.Choices
{
    class KhansFavor : IActionChoice
    {
        private readonly Player _player;
        private readonly KhansFavorSpace _space;
        private readonly ResourceType _resourceType;
        private readonly Die _die;

        public KhansFavor(Player player, KhansFavorSpace space, ResourceType resourceType, Die die)
        {
            _player = player;
            _space = space;
            _resourceType = resourceType;
            _die = die;
        }

        public void Execute()
        {
            _player.PlayDice(new[] { _die }, _space);
            var reward = new Reward { Camel = 2, Gold = _resourceType == Gold ? 1 : 0, Silk = _resourceType == Silk ? 1 : 0, Pepper = _resourceType == Pepper ? 1 : 0 };
            _player.GainReward(reward, _space.Description);
            _player.HasTakenActionThisTurn = true;
        }

        public bool IsValid()
        {
            return _player.CanPlayInActionSpace(_space)
                   && _space.Dice.Count < 4
                   && _player.AvailableDice.Any(d => d.Value >= _space.MinimumValue);
        }
    }
}

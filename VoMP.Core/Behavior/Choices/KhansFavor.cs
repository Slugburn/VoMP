using System;
using System.Linq;
using VoMP.Core.Actions;
using static VoMP.Core.ResourceType;

namespace VoMP.Core.Behavior.Choices
{
    public class KhansFavor : ISpaceActionChoice
    {
        private readonly Player _player;

        public KhansFavor(Player player)
        {
            _player = player;
            Space = player.Game.GetActionSpace<KhansFavorSpace>();
        }

        public KhansFavorSpace Space { get; }
        public ResourceType ResourceType { get; set; }
        public Die Die { get; set; }

        public void Execute()
        {
            if (Die == null)
                throw new InvalidOperationException("Die value has not been specified");
            _player.PlayDice(new[] {Die}, Space);
            var reward = GetReward();
            _player.GainReward(reward, Space.Description);
            _player.HasTakenActionThisTurn = true;
        }

        public Cost GetCost()
        {
            return Cost.None;
        }

        public Reward GetReward()
        {
            return new Reward
            {
                Camel = 2,
                Gold = ResourceType == Gold ? 1 : 0,
                Silk = ResourceType == Silk ? 1 : 0,
                Pepper = ResourceType == Pepper ? 1 : 0
            };
        }

        public bool IsValid()
        {
            return _player.CanPlayInActionSpace(Space)
                   && Space.DiceCount < 4
                   && _player.AvailableDice.Any(d => d.Value >= Space.MinimumValue);
        }
    }
}
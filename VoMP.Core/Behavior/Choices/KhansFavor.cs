using System;
using System.Collections.Generic;
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

        public ResourceType ResourceType { get; set; }
        public Die Die { get; set; }
        public int MinimumValue => ((KhansFavorSpace) Space).MinimumValue;
        public ActionSpace Space { get; }

        public void Execute()
        {
            if (Die == null)
                throw new InvalidOperationException("Die value has not been specified");
            _player.PlayDice(new[] {Die}, Space);
            var reward = Reward;
            _player.GainReward(reward, Space.Description);
            _player.HasTakenActionThisTurn = true;
        }

        public Cost Cost => Cost.None;

        public Reward Reward => RewardFor(ResourceType);

        public IList<Die> Dice => new[] {Die};
        public int Value => 6;

        public bool IsValid()
        {
            return _player.CanPlayInActionSpace(Space)
                   && Space.DiceCount < 4
                   && _player.AvailableDice.Any(d => !d.HasValue || d.Value >= MinimumValue);
        }

        private static Reward RewardFor(ResourceType resourceType)
        {
            switch (resourceType)
            {
                case Gold:
                    return Reward.Of.Camel(2).And.Gold(1);
                case Silk:
                    return Reward.Of.Camel(2).And.Silk(1);
                case Pepper:
                    return Reward.Of.Camel(2).And.Pepper(1);
                default:
                    throw new InvalidOperationException("Invalid ResourceType");
            }
        }
    }
}
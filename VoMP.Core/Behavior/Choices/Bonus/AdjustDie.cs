using System.Collections.Generic;

namespace VoMP.Core.Behavior.Choices.Bonus
{
    public class AdjustDie : IActionChoice
    {
        private readonly Player _player;

        public AdjustDie(Player player)
        {
            _player = player;
        }

        public Die Die { get; set; }
        public int Direction { get; set; }

        public void Execute()
        {
            _player.PayCost(Cost, $"to adjust {Die} upward" );
            Die.Adjust(Direction);
        }

        public Cost Cost { get; } =  Cost.Of.Camel(2);

        public Reward Reward { get; } = Reward.None;

        public bool IsValid()
        {
            return true;
        }
    }
}

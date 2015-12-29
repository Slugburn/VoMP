namespace VoMP.Core.Behavior.Choices.Bonus
{
    public class RerollDie : IActionChoice
    {
        private readonly Player _player;

        public RerollDie(Player player)
        {
            _player = player;
        }

        public Die Die { get; set; }

        public void Execute()
        {
            _player.PayCost(Cost, $"reroll {Die}");
            Die.Roll();
            _player.Output($"rolls {Die}");
        }

        public Cost Cost { get; } = new Cost {Camel = 1};

        public bool IsValid()
        {
            return true;
        }
    }
}

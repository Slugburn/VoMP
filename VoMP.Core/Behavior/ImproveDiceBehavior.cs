using System.Linq;
using VoMP.Core.Actions;
using VoMP.Core.Behavior.Choices;
using VoMP.Core.Behavior.Choices.Bonus;
using VoMP.Core.Extensions;

namespace VoMP.Core.Behavior
{
    public static class ImproveDiceBehavior
    {
        public static IActionChoice ImproveDice(AiState state, ActionSpace space, int targetValue)
        {
            var availableDice = state.GetDiceAvailableFor(space);
            var oneLessThanTarget = availableDice.GetLowestDice(space.RequiredDice, targetValue - 1);
            if (oneLessThanTarget.Count == space.RequiredDice)
            {
                var adjustDie = AdjustDie(state, oneLessThanTarget.GetLowestDie(), 1);
                if (adjustDie != null) return adjustDie;
            }
            if (availableDice.Any(d => d.Value == 1))
            {
                var rerollDie = RerollDie(state);
                if (rerollDie != null) return rerollDie;
            }
            var buyBlackDie = BuyBlackDie(state);
            return buyBlackDie;
        }

        private static IActionChoice RerollDie(AiState state)
        {
            var rerollDie = new RerollDie(state.Player) {Die = state.Player.AvailableDice.First(d => d.Value == 1)};
            if (rerollDie.IsValid())
                return state.PlayerCanAfford(rerollDie) ? rerollDie : null;
            return null;
        }

        private static IActionChoice AdjustDie(AiState state, Die die, int direction)
        {
            var adjustDie = new AdjustDie(state.Player) {Die = die, Direction = direction};
            if (adjustDie.IsValid())
                return state.PlayerCanAfford(adjustDie) ? adjustDie : GenerateResourcesBehavior.GenerateResources(state, adjustDie.Cost, "adjust die");
            return null;
        }

        public static IActionChoice BuyBlackDie(AiState state)
        {
            var buyBlackDie = new BuyBlackDie(state.Player);
            if (!buyBlackDie.IsValid()) return null;
            return state.PlayerCanAfford(buyBlackDie) ? buyBlackDie : GenerateResourcesBehavior.GenerateResources(state, buyBlackDie.Cost, "buy black die");
        }
    }
}

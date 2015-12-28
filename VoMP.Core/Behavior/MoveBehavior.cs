using System.Linq;
using VoMP.Core.Actions;
using VoMP.Core.Behavior.Choices;
using VoMP.Core.Behavior.Choices.Bazaar;
using VoMP.Core.Behavior.Choices.Bonus;
using VoMP.Core.Extensions;

namespace VoMP.Core.Behavior
{
    class MoveBehavior
    {
        public static IActionChoice Move(AiState state)
        {
            var nextMove = state.NextMove;
            if (nextMove == null) return null;
            var moveNeeded = nextMove.Count;

            // Reserve resources needed to complete next move
            var moveCost = nextMove.GetCost();
            var reason = $"move {moveNeeded} spaces from {nextMove.First().Start} to {nextMove.Last().End}";
            var reserveResourcesChoice = new ReserveResourcesChoiceParam(() => DoMove(state), reason) {Cost = moveCost};
            return state.MakeChoiceWithReservedResources(reserveResourcesChoice);
        }

        private static IActionChoice DoMove(AiState state)
        {
            var moveNeeded = state.NextMove.Count;
            return moveNeeded > 1 ? MoveMany(state) : (MoveOne(state) ?? MoveMany(state));
        }

        private static IActionChoice MoveMany(AiState state)
        {
            var player = state.Player;
            var nextMove = state.NextMove;
            var moveNeeded = nextMove.Count;

            var cityActions = state.GetValidCityActions(ResourceType.Move);
            var bestAction = state.ChooseBestAction(cityActions, (r, c) => r.Move >= moveNeeded);
            if (bestAction!= null)
                return bestAction;

            var travel = new Travel(player) {Path = nextMove};

            if (!travel.IsValid()) return null;

            var dice = player.GetDiceAvailableFor(travel.Space).GetLowestDice(2, moveNeeded);
            if (dice.Count == 0) return null;
            if (dice.Count == 1) return ImproveDiceBehavior.ImproveDice(state, travel.Space, moveNeeded);

            travel.Dice = dice;

            var cost = travel.GetCost().Add(nextMove.GetCost());
            if (player.CanPay(cost)) return travel;

            // Need to be able to generate resources in order to travel
            var p = new ReserveResourcesChoiceParam(()=> GenerateResourcesBehavior.GenerateResources(state, travel.GetCost(), "travel"), "travel") {Dice = dice};
            return state.MakeChoiceWithReservedResources(p);
        }

        private static IActionChoice MoveOne(AiState state)
        {
            var nextMove = state.NextMove;
            var moveCost = nextMove.GetCost();

            // does one of our current contracts allow us to move 1?
            var completeContract = CompleteContractBehavior.CompleteContract(state, x => x.Reward.Move > 0);
            if (completeContract != null) return completeContract;

            // can we use the gold bazaar to move?
            var actionChoice = MoveUsingGoldBazaar(state, moveCost);
            if (actionChoice != null) return actionChoice;

            // are there any move contracts available to us?
            return TakeContractsBehavior.TakeContracts(state, c => c.Reward.Move > 0);
        }

        private static IActionChoice MoveUsingGoldBazaar(AiState state, Cost moveCost)
        {
            var player = state.Player;
            var goldBazaar = new GoldBazaar(player) {Value = 5};
            if (!goldBazaar.IsValid()) return null;
            var dice = player.AvailableDice.GetLowestDice(3, 5);
            goldBazaar.Dice = dice;
            if (dice.Count < 2) return null;
            var cost = moveCost.Add(player.GetOccupancyCost(goldBazaar.Space, dice));
            if (dice.Count == 2)
            {
                if (!player.CanPay(cost)) return null;
                var p = new ReserveResourcesChoiceParam(() => ImproveDiceBehavior.ImproveDice(state, goldBazaar.Space, 5),"visit the Gold Bazaar") {Dice = dice};
                return state.MakeChoiceWithReservedResources(p);
            }
            return player.CanPay(cost) ? goldBazaar : GenerateResourcesBehavior.GenerateResources(state, cost, "visit the gold bazaar");
        }
    }
}

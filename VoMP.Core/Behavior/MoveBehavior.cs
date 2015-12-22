using System.Linq;
using VoMP.Core.Actions;
using VoMP.Core.Behavior.Choices;
using VoMP.Core.Behavior.Choices.Bazaar;
using VoMP.Core.Extensions;

namespace VoMP.Core.Behavior
{
    class MoveBehavior
    {
        public static IActionChoice Move(AiState state)
        {
            var nextMove = state.NextMove;
            if (nextMove == null) return null;
            
            // Reserve resources needed to complete next move
            state.ReserveResources(nextMove.GetCost());

            var moveNeeded = nextMove.Count;
            if (moveNeeded > 1) return Travel(state);

            return MoveOne(state) ?? Travel(state);
        }

        private static IActionChoice Travel(AiState state)
        {
            var player = state.Player;
            var nextMove = state.NextMove;
            var moveNeeded = nextMove.Count;

            var travel = new Travel(player) {Path = nextMove};

            if (!travel.IsValid()) return null;

            var dice = player.GetDiceAvailableFor(travel.Space).GetLowest(2, d=>d.Value >= moveNeeded);
            if (dice.Count == 0) return null;
            if (dice.Count == 1) return ImproveDice(state, travel.Space, moveNeeded);

            travel.Dice = dice;

            // Must be able to pay travel cost + occupancy cost + move cost
            var travelCost = travel.GetCost(nextMove);
            var occupancyCost = player.GetOccupancyCost(travel.Space, dice);
            var moveCost = nextMove.GetCost();
            var totalCost = travelCost.Add(occupancyCost).Add(moveCost);
            if (player.CanPay(totalCost)) return travel;

            // Need to be able to generate resources in order to travel
            if (player.AvailableDice.Count > dice.Count)
                state.ReserveDice(dice);
            return GenerateResourcesBehavior.GenerateResources(state, totalCost);
        }

        private static IActionChoice MoveOne(AiState state)
        {
            var nextMove = state.NextMove;
            var moveCost = nextMove.GetCost();

            // does one of our current contracts allow us to move 1?
            var completeContract = CompleteContractBehavior.CompleteContract(state, x => x.Reward.Move > 0, moveCost);
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
            var dice = player.AvailableDice.GetLowest(3, d => d.Value >= 5);
            goldBazaar.Dice = dice;
            if (dice.Count < 2) return null;
            var cost = moveCost.Add(player.GetOccupancyCost(goldBazaar.Space, dice));
            if (dice.Count == 2)
            {
                var improveDice = ImproveDice(state, goldBazaar.Space, 5);
                return player.CanPay(cost) ?  improveDice : null;
            }
            return player.CanPay(cost) ? goldBazaar : GenerateResourcesBehavior.GenerateResources(state, cost);
        }

        private static IActionChoice ImproveDice(AiState state, SpaceAction space, int targetValue)
        {
            var player = state.Player;
            var availableDice = state.GetDiceAvailableFor(space);
            var oneLessThanTarget = availableDice.GetLowest(space.RequiredDice, d => d.Value >= targetValue - 1);
            if (oneLessThanTarget.Count == space.RequiredDice)
            {
                var adjustDie = new AdjustDie(player) {Die = oneLessThanTarget.GetLowest(), Direction = 1};
                if (adjustDie.IsValid() && state.PlayerCanPay(adjustDie.Cost)) return adjustDie;
            }
            if (availableDice.Any(d => d.Value == 1))
            {
                var rerollDie = new RerollDie(player) {Die = player.AvailableDice.First(d => d.Value == 1)};
                if (rerollDie.IsValid())
                    return state.PlayerCanPay(rerollDie.Cost) ? rerollDie : GenerateResourcesBehavior.GenerateResources(state, rerollDie.Cost);
            }
            var buyBlackDie = new BuyBlackDie(state.Player);
            if (buyBlackDie.IsValid())
                return state.PlayerCanPay(buyBlackDie.Cost) ? buyBlackDie : GenerateResourcesBehavior.GenerateResources(state, buyBlackDie.Cost);
            return null;
        }
    }
}

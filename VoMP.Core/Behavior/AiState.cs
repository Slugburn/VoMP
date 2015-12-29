using System;
using System.Collections.Generic;
using System.Linq;
using VoMP.Core.Actions;
using VoMP.Core.Behavior.Choices;
using VoMP.Core.CityCards;
using VoMP.Core.Extensions;

namespace VoMP.Core.Behavior
{
    public class AiState
    {
        public AiState(Player player)
        {
            Player = player;
            BestPath = player.GetBestPath();
            NextMove = BestPath != null ? GetMoves(BestPath, player.TradingPosts).First() : null;
        }

        public Player Player { get; }

        public List<Route> BestPath { get; }
        public List<Route> NextMove { get; }

        public Cost ReservedResources { get; private set; } = Cost.None;
        public List<Die> ReservedDice { get; } = new List<Die>();

        public Cost Shortfall { get; set; }

        public List<Die> GetDiceAvailableFor(ActionSpace space) => Player.GetDiceAvailableFor(space).Except(ReservedDice).ToList();

        public List<Die> AvailableDice => Player.AvailableDice.Except(ReservedDice).ToList();


        private static IEnumerable<List<Route>> GetMoves(List<Route> path, ISet<Location> tradingPosts)
        {
            var list = new List<Route>();
            foreach (var route in path)
            {
                list.Add(route);
                if (!route.End.IsTradeLocation() || tradingPosts.Contains(route.End)) continue;
                yield return list;
                list = new List<Route>();
            }
        }

        public void ClearResourceReserves()
        {
            ReservedResources = Cost.None;
        }

        public bool PlayerCanPay(Cost cost)
        {
            if (cost == null) return false;
            var adjustedCost = cost.AllowingFor(ReservedResources);
            return Player.CanPay(adjustedCost);
        }

        public Cost GetShortfall(Cost cost)
        {
            return Player.Resources.GetShortfall(cost.AllowingFor(ReservedResources));
        }

        public IActionChoice MakeChoiceWithReservedResources(ReserveResourcesChoiceParam p)
        {
            if (p.Cost != null && p.Cost.Rating > 0)
            {
                Player.Debug($"reserves {p.Cost} needed to {p.Reason}");
                ReserveResources(p.Cost);
            }
            if (p.Dice != null)
            {
                Player.Debug($"reserves {p.Dice.ToDelimitedString("")} needed to {p.Reason}");
                ReservedDice.AddRange(p.Dice);
            }
            var choice = p.MakeChoice();
            if (choice != null) return choice;
            if (p.Cost != null)
                UnreserveResources(p.Cost);
            p.Dice?.ForEach(d => ReservedDice.Remove(d));
            return null;
        }

        public void ReserveResources(Cost cost)
        {
            ReservedResources = ReservedResources.Add(cost);
        }

        public void UnreserveResources(Cost cost)
        {
            ReservedResources =  ReservedResources.Subtract(cost);
        }

        public List<CityAction> GetValidCityActions(ResourceType resourceType)
        {
            var player = Player;
            var cityActions = player.CityActions
                .Where(space => !space.IsOccupied && space.Card.CanGenerate(player, resourceType))
                .SelectMany(action => CreateCityAction(action, resourceType))
                .Where(a => a.IsValid())
                .ToList();
            return cityActions;
        }

        private IEnumerable<CityAction> CreateCityAction(LargeCityAction space, ResourceType resourceType)
        {
            var availableDice = GetDiceAvailableFor(space);
            if (!availableDice.Any()) yield break;
            for (var value = 1; value <= availableDice.MaxValue(); value++)
            {
                var die = availableDice.GetLowestDie(value);
                var optionCityCard = space.Card as OptionCityCard;
                if (optionCityCard == null)
                    yield return new CityAction(Player, space) {Card = space.Card,  Value = value, Die = die};
                else
                    foreach (var card in optionCityCard.GetOptions(Player, resourceType))
                    {
                        yield return new CityAction(Player, space) { Card = card, Value = value, Die = die };
                    }
            }
        }

        public IActionChoice ChooseBestAction(IEnumerable<ISpaceActionChoice> choices, Func<Reward,Cost,bool> rewardMeetsShortfall)
        {
            var possibleActions = choices.Where(a => a != null && a.IsValid()).ToList();
            if (!possibleActions.Any()) return null;
            var meetsRequirement = possibleActions.Where(a => a.GetReward() != null && rewardMeetsShortfall(a.GetReward(), Shortfall)).ToList();
            var canAfford = possibleActions.Where(a => PlayerCanPay(a.GetCost())).ToList();
            var best = meetsRequirement.Intersect(canAfford)
                .OrderByDescending(a => a.GetReward().Rating - a.GetCost().Rating)
                .FirstOrDefault();
            if (best != null) return best;
            var bestAffordable = canAfford.OrderByDescending(a => a.GetReward().Rating - a.GetCost().Rating).FirstOrDefault();
            return bestAffordable;
        }

        public Cost GetOutstandingCosts()
        {
            return NextMove.GetCost().Add(Player.Contracts.Select(c=>c.Cost).Total()).Add(Travel.GetTravelCost(NextMove));
        }

        public Cost GetOutstandingShortfall()
        {
            return Player.Resources.GetShortfall(GetOutstandingCosts());
        }

        public Cost GetOccupancyCost(ISpaceActionChoice choice)
        {
            return Player.GetOccupancyCost(choice.Space, choice.Dice, choice.Value);
        }

        public int GetBestDiceValue(int count)
        {
            var dice = AvailableDice;
            if (dice.Any(d => !d.HasValue))
                return 6;
            return dice.GetHighestDice(count).MinValue();
        }
    }
}
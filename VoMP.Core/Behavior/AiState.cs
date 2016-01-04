using System;
using System.Collections.Generic;
using System.Linq;
using VoMP.Core.Actions;
using VoMP.Core.Behavior.Choices;
using VoMP.Core.CityCards;
using VoMP.Core.Extensions;
using Action = System.Action;

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

        public List<Die> AvailableDice => Player.AvailableDice.Except(ReservedDice).ToList();

        public List<Die> GetDiceAvailableFor(ActionSpace space) => Player.GetDiceAvailableFor(space).Except(ReservedDice).ToList();

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

        public Cost GetShortfall(Cost cost)
        {
            return Player.Resources.GetShortfall(cost.AllowingFor(ReservedResources));
        }

        public IDisposable ReserveResources(Cost cost, string reason)
        {
            Player.Debug($"reserves {cost} needed to {reason}");
            ReservedResources = ReservedResources.Add(cost);
            return new Disposable(() => ReservedResources = ReservedResources.Subtract(cost));
        }

        public IDisposable ReserveDice(IEnumerable<Die> dice, string reason)
        {
            var list = dice as IList<Die> ?? dice.ToList();
            Player.Debug($"reserves {list.ToDelimitedString("")} needed to {reason}");
            ReservedDice.AddRange(list);
            return new Disposable(() => list.ForEach(d => ReservedDice.Remove(d)));
        }

        public List<CityAction> GetValidCityActions(ResourceType resourceType)
        {
            var player = Player;
            var cityActions = player.CityActions
                .Where(space => !space.IsOccupied && space.Card.CanGenerate(player, resourceType))
                .SelectMany(action => CreateCityAction(action))
                .Where(a => a.IsValid())
                .ToList();
            return cityActions;
        }

        public List<CityAction> GetValidCityActions()
        {
            var player = Player;
            var cityActions = player.CityActions
                .Where(space => !space.IsOccupied)
                .SelectMany(CreateCityAction)
                .Where(a => a.IsValid())
                .ToList();
            return cityActions;
        }

        private IEnumerable<CityAction> CreateCityAction(LargeCityAction space)
        {
            var availableDice = GetDiceAvailableFor(space);
            if (!availableDice.Any()) yield break;
            for (var value = 1; value <= availableDice.MaxValue(); value++)
            {
                var die = availableDice.GetLowestDie(value);
                var optionCityCard = space.Card as OptionCityCard;
                if (optionCityCard == null)
                    yield return new CityAction(Player, space) {Card = space.Card, Value = value, Die = die};
                else
                    foreach (var card in optionCityCard.GetOptions(Player))
                    {
                        yield return new CityAction(Player, space) {Card = card, Value = value, Die = die};
                    }
            }
        }

        public IActionChoice ChooseBestAction(IEnumerable<ISpaceActionChoice> choices, Cost cost)
        {
            var possibleActions = choices.Where(a => a != null && a.IsValid()).ToList();
            if (!possibleActions.Any()) return null;
            var realizedActions = possibleActions.Select(c => new RealizedActionChoice(c, RealizeReward(c.Reward), cost)).ToList();

            var meetsRequirement = realizedActions.Where(a => a.Reward != null && cost.Subtract(a.Reward).Rating == 0).ToList();
            var canAfford = realizedActions.Where(PlayerCanAfford).ToList();
            var best = meetsRequirement.Intersect(canAfford)
                .OrderByDescending(a => a.Rating)
                .FirstOrDefault();
            if (best != null) return best.Choice;
            var bestAffordable = canAfford.OrderByDescending(a => a.Rating).FirstOrDefault();
            return bestAffordable?.Choice;
        }

        public Cost GetOutstandingCosts()
        {
            var contractCost = Player.Contracts.Select(c => c.Cost).Total();
            if (NextMove == null) return contractCost;
            var cost = NextMove.GetCost().Add(contractCost).Add(Travel.GetTravelCost(NextMove));
            return cost;
        }

        public Cost GetOutstandingShortfall()
        {
            var outstandingCosts = GetOutstandingCosts();
            var shortfall = Player.Resources.GetShortfall(outstandingCosts);
            return shortfall;
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

        private Reward RealizeReward(Reward reward)
        {
            while (true)
            {
                if (reward.OtherCityBonus + reward.TradingPostBonus + reward.Good + reward.UniqueGood == 0) return reward;
                var realized = reward.GetConcreteRewards();
                var behavior = Player.Behavior;
                if (reward.OtherCityBonus > 0)
                    realized = realized.Add(behavior.ChooseOtherCityBonus(Player).Reward);
                if (reward.TradingPostBonus > 0)
                {
                    var cityBonuses = behavior.ChooseTradingPostBonuses(Player, reward.TradingPostBonus);
                    realized = cityBonuses.Aggregate(realized, (current, cb) => current.Add(cb.Reward));
                }
                if (reward.Good > 0)
                    realized = realized.Add(behavior.ChooseGoodsToGain(Player, reward.Good));
                if (reward.UniqueGood > 0)
                    realized = realized.Add(behavior.ChooseUniqueGoodsToGain(Player, reward.UniqueGood));
                reward = realized;
            }
        }

        public Cost CostIncludingNextMove(IExchange exchange)
        {
            var cost = exchange.Cost;
            if (cost == null) return null;
            if (exchange.Reward.Move > 0 && NextMove != null)
                cost = cost.Add(NextMove.First().Cost);
            return cost;
        }

        private class Disposable : IDisposable
        {
            private readonly Action _onDisposal;

            public Disposable(Action onDisposal)
            {
                _onDisposal = onDisposal;
            }

            public void Dispose()
            {
                _onDisposal();
            }
        }

        public class RealizedActionChoice : IExchange
        {
            private readonly Cost _targetCost;

            public RealizedActionChoice(ISpaceActionChoice choice, Reward reward, Cost targetCost)
            {
                _targetCost = targetCost;
                Choice = choice;
                Reward = reward;
            }

            public ISpaceActionChoice Choice { get; set; }
            public Cost Cost => Choice.Cost;
            public Reward Reward { get; set; }
            public int Rating => Reward.Rating - Choice.Cost.Rating - Choice.Space.RequiredDice*3;

            public bool CanFulfill(Cost cost)
            {
                return Reward.Camel >= cost.Camel
                       && Reward.Coin >= cost.Coin
                       && Reward.Gold >= cost.Gold
                       && Reward.Silk >= cost.Silk
                       && Reward.Pepper >= cost.Pepper
                       && Reward.Gold + Reward.Silk + Reward.Pepper >= cost.Good
                       && Reward.Vp >= cost.Vp
                       && Reward.Move >= cost.Move;
            }
        }

        public bool PlayerCanAfford(IExchange exchange)
        {
            var cost = CostIncludingNextMove(exchange);
            if (cost == null) return false;
            if (!Player.CanPay(cost)) return false;
            // You have to spend money to make money: add reward before checking to see if we can still cover reserved resources after making exchange
            var adjustedResources = Player.Resources.Add(exchange.Reward.GetResources());
            var adjustedCost = cost.AllowingFor(ReservedResources);
            return adjustedResources.CanPay(adjustedCost);
        }
    }
}
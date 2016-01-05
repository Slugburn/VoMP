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

        public Cost GetShortfall(Cost cost)
        {
            return Player.Resources.GetShortfall(cost.AllowingFor(ReservedResources));
        }

        public IDisposable ReserveResources(Cost cost, string reason)
        {
            if (cost.Rating >0)
                Player.Debug($"reserves {cost} needed to {reason}");
            ReservedResources = ReservedResources.Add(cost);
            return new Disposable(() => ReservedResources = ReservedResources.Subtract(cost));
        }

        public IDisposable ReserveDice(IEnumerable<Die> dice, string reason)
        {
            var list = dice as IList<Die> ?? dice.ToList();
            if (list.Any())
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

        public IActionChoice ChooseBestAction(IEnumerable<IActionChoice> choices, Cost shortfall)
        {
            var possibleActions = choices.Where(a => a != null && a.IsValid()).ToList();
            if (shortfall.Move > 0)
                possibleActions = possibleActions.Where(a => a.Reward.Move == shortfall.Move).ToList();
            if (!possibleActions.Any()) return null;
            var realizedActions = possibleActions
                .Select(c => new RealizedActionChoice(c, RealizeReward(c.Reward), RealizeCost(c.Cost), shortfall))
                .Where(c=>c.Rating >= 0)
                .ToList();
            var bestAction = realizedActions
                .Where(x => x.EffectivenessRating == 0)
                .OrderByDescending(x=>x.Rating)
                .FirstOrDefault();
            var bestChoice = TryChoice(bestAction);
            if (bestChoice != null) return bestChoice;

            var meetsRequirements = realizedActions
                .Where(x => x.EffectivenessRating == 0)
                .OrderBy(x => x.Cost.Rating).ThenByDescending(x => x.Rating)
                .FirstOrDefault();
            var choiceThatMeetsRequirements = TryChoice(meetsRequirements);

            var canAfford = realizedActions.Where(PlayerCanAfford).ToList();
                var mostEffectiveRating = canAfford.Any() ? canAfford.Min(a => a.EffectivenessRating) : int.MaxValue;
                var mostEffective = canAfford
                    .Where(a => a.EffectivenessRating == mostEffectiveRating)
                    .OrderByDescending(a => a.Rating)
                    .FirstOrDefault();
            var mostEffectiveChoice = mostEffective?.Choice;

            if (choiceThatMeetsRequirements == null) return mostEffectiveChoice;
            if (mostEffectiveChoice == null) return choiceThatMeetsRequirements;
            return mostEffectiveChoice.Reward.Rating > choiceThatMeetsRequirements.Reward.Rating ? mostEffectiveChoice : choiceThatMeetsRequirements;
        }

        private IActionChoice TryChoice(RealizedActionChoice choice)
        {
            if (choice == null) return null;
            var bestChoice = choice.Choice;
            if (PlayerCanAfford(choice)) return bestChoice;
            using (ReserveDice(bestChoice is ISpaceActionChoice ? ((ISpaceActionChoice) bestChoice).Dice : new Die[0], bestChoice.ToString()))
            {
                var bestCost = CostIncludingNextMove(bestChoice);
                var generateResources = GenerateResourcesBehavior.GenerateResources(this, bestCost, bestChoice.ToString());
                if (generateResources != null && generateResources.Reward.CanFulfill(bestCost)) return generateResources;
            }
            return null;
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

        private Cost RealizeCost(Cost cost)
        {
            var realized = cost.GetConcreteCosts();
            var behavior = Player.Behavior;
            if (cost.Good == 0) return realized;
            var toGenerate = GetShortfall(cost).Good;
            var toSpend = behavior.ChooseGoodsToPay(Player, Cost.Of.Good(cost.Good - toGenerate));
            realized = realized.Add(Cost.Of.Pepper(toGenerate)).Add(toSpend);
            return realized;
        }

        public Cost CostIncludingNextMove(IExchange exchange)
        {
            var cost = exchange.Cost;
            if (cost == null) return null;
            var moves = exchange.Reward.Move;
            if (moves > 0 && NextMove != null)
                cost = cost.Add(NextMove.Take(moves).GetCost());
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
            private readonly Cost _shortfall;
            private readonly int _requiredDice;

            public RealizedActionChoice(IActionChoice choice, Reward reward, Cost cost, Cost shortfall)
            {
                _shortfall = shortfall;
                Choice = choice;
                Reward = reward;
                Cost = cost;
                EffectivenessRating = shortfall.Subtract(Reward).Rating;
                var spaceAction = choice as ISpaceActionChoice;
                _requiredDice = spaceAction?.Space.RequiredDice ?? 0;
            }

            public IActionChoice Choice { get; set; }
            public Cost Cost { get; set; }
            public Reward Reward { get; set; }
            public int Rating => Reward.Rating - Choice.Cost.Rating - _requiredDice*3;

            // Lower is better
            public int EffectivenessRating { get; }
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
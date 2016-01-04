using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using VoMP.Core.Actions;
using VoMP.Core.Behavior;
using VoMP.Core.CityCards;

namespace VoMP.Core.Tests
{
    [TestFixture]
    public class AiTest
    {
        [Test]
        public void ChooseBestActionToPartiallyFulfillShortfall()
        {
            new TestScenario()
                .SetResources(Reward.Of.Camel(2).And.Coin(7))
                .SetContracts(new Contract(Cost.Of.Camel(2).And.Silk(2).And.Pepper(3), Reward.Of.Move(1).And.Vp(5)))
                .SetAvailableDice(6, 6, 6, 6, 6)
                .TryToCompleteContracts()
                .VerifyResources(new ResourceBag {Camel = 2, Coin = 7, Silk = 4});
        }

        [Test]
        public void UseCityBonusesToGenerateResources()
        {
            var bonusPerCoinCard = new ExchangeCityCard(Cost.Of.Coin(1), Reward.Of.TradingPostBonus(1));
            new TestScenario()
                .BuildTradingPostForCityBonus(r => r.Coin == 5)
                .BuildTradingPostForCityBonus(r => r.Camel == 1 && r.Coin == 3)
                .BuildTradingPostForCityBonus(r => r.OtherCityBonus == 1)
                .ConfigureLargeCity(Location.Alexandria, bonusPerCoinCard)
                .SetAvailableDice(1, 4, 6)
                .SetResources(Reward.Of.Coin(3))
                .SetContracts()
                .SetNextMove(new Route(Location.Alexandria, Location.Kochi, Cost.Of.Coin(10)))
                // AI should choose to play on Alexandria space and pay 3 coins to generate trading post bonuses for a total of 13 coins and 1 camel
                .GenerateResources(Cost.Of.Coin(10))
                .VerifyResources(new ResourceBag {Coin = 13, Camel = 1});
        }

        [Test]
        public void GenerateResourcesHolistically()
        {
            new TestScenario()
                .ConfigureLargeCity(Location.Alexandria, new ExchangeCityCard(Cost.Of.Camel(3), Reward.Of.Gold(1).And.Silk(1).And.Pepper(1)))
                .ConfigureLargeCity(Location.Moscow, new DieRangeCityCard(1, 5, Reward.Of.Gold(1), 6, 6, Reward.Of.Gold(3)))
                .SetResources(Reward.Of.Camel(9))
                .SetContracts(new Contract(Cost.Of.Camel(3).And.Gold(1).And.Silk(1).And.Pepper(1), Reward.Of.Die(1).And.Vp(5)))
                .SetAvailableDice(1, 2, 3, 4, 6)
                .TryToCompleteContracts()
                .VerifyResources(new ResourceBag() {Camel = 3, Gold = 2, Silk = 2, Pepper = 2});
        }

        public class TestScenario
        {
            private readonly Game _game;
            private readonly Player _player;
            private readonly AiState _state;

            public TestScenario()
            {
                _game = new Game();
                _game.SetUp();
                _player = _game.StartPlayer;
                _player.AvailableDice = new List<Die>();
                _state = new AiState(_player);
                _player.Behavior = new AiBehavior {State = _state};
            }

            public TestScenario SetAvailableDice(params int[] values)
            {
                _player.AvailableDice = values.Select(x=>Die.Create(_player.Color, x)).ToList();
                return this;
            }

            public Location GetCityBonusLocation(Func<Reward, bool> condition)
            {
                return _game.GetCityBonusLocation(condition);
            }

            public TestScenario BuildTradingPost(Location location)
            {
                _player.BuildTradingPost(location);
                return this;
            }

            public TestScenario ConfigureLargeCity(Location location, ICityCard card)
            {
                var mapSpace = _game.GetMapSpace(location);
                mapSpace.Actions.Clear();
                mapSpace.Actions.Add(new LargeCityAction(location, card));
                _player.BuildTradingPost(location);
                return this;
            }

            public TestScenario SetResources(Reward resources)
            {
                _player.Resources.Zero();
                _player.GainReward(resources, "initial");
                return this;
            }

            public TestScenario SetNextMove(params Route[] move)
            {
                _state.NextMove.Clear();
                _state.NextMove.AddRange(move);
                return this;
            }

            public TestScenario GenerateResources(Cost cost)
            {
                var choice = GenerateResourcesBehavior.GenerateResources(_state, cost, "test");
                choice.Execute();
                return this;
            }

            public TestScenario VerifyResources(ResourceBag resources)
            {
                Assert.That(_player.Resources, Is.EqualTo(resources));
                return this;
            }

            public TestScenario BuildTradingPostForCityBonus(Func<Reward, bool> condition)
            {
                return BuildTradingPost(GetCityBonusLocation(condition));
            }

            public TestScenario SetContracts(params  Contract[] contracts)
            {
                _player.Contracts.Clear();
                _player.Contracts.AddRange(contracts);
                return this;
            }

            public TestScenario TryToCompleteContracts()
            {
                var choice = CompleteContractBehavior.CompleteContract(_state,c=>true);
                choice.Execute();
                return this;
            }
        }
    }
}

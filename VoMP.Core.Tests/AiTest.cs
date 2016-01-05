using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using VoMP.Core.Actions;
using VoMP.Core.Behavior;
using VoMP.Core.CityCards;
using static VoMP.Core.Location;

namespace VoMP.Core.Tests
{
    [TestFixture]
    public class AiTest
    {
        public class TestScenario
        {
            private readonly Game _game;
            private readonly Player _player;
            private Route[] _nextMove;

            public TestScenario()
            {
                _game = new Game();
                _game.SetUp();
                _player = _game.StartPlayer;
                _player.AvailableDice = new List<Die>();
                _player.Behavior = new AiBehavior {State = new AiState(_player)};

                _nextMove = new Route[0];
            }

            public TestScenario SetAvailableDice(params int[] values)
            {
                _player.AvailableDice = values.Select(x => Die.Create(_player.Color, x)).ToList();
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
                _nextMove = move;
                return this;
            }

            public TestScenario GenerateResources(Cost cost)
            {
                var choice = GenerateResourcesBehavior.GenerateResources(GetState(), cost, "test");
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

            public TestScenario SetContracts(params Contract[] contracts)
            {
                _player.Contracts.Clear();
                _player.Contracts.AddRange(contracts);
                return this;
            }

            public TestScenario TryToCompleteContracts()
            {
                var choice = CompleteContractBehavior.CompleteContract(GetState(), c => true);
                choice.Execute();
                _player.HasTakenActionThisTurn = false;
                return this;
            }

            private AiState GetState()
            {
                var state = new AiState(_player);
                state.NextMove.AddRange(_nextMove);
                ((AiBehavior) _player.Behavior).State = state;
                return state;
            }

            public TestScenario SetGoals(Objective objective1, Objective objective2)
            {
                _player.Objectives = new List<Objective> { objective1, objective2 };
                return this;
            }

            public TestScenario SetLocation(Location location)
            {
                _player.MovePawn(Venezia, location);
                return this;
            }

            public TestScenario VerifyLocation(Location location)
            {
                Assert.That(_player.GetPawnLocation(), Is.EqualTo(location));
                return this;
            }
        }

        [Test]
        public void ChooseBestActionToPartiallyFulfillShortfall()
        {
            new TestScenario()
                .SetResources(Reward.Of.Camel(2).And.Coin(7))
                .SetContracts(new Contract(Cost.Of.Camel(2).And.Silk(2).And.Pepper(3), Reward.Of.Move(1).And.Vp(5)))
                .SetAvailableDice(6, 6, 6, 6, 6)
                .TryToCompleteContracts()
                .VerifyResources(new ResourceBag {Camel = 3, Coin = 8, Silk = 3});
        }

        [Test]
        public void CompleteContractToGenerateResources()
        {
            new TestScenario()
                .SetResources(Reward.Of.Camel(2).And.Gold(3).And.Pepper(2))
                .SetContracts(new Contract(Cost.Of.Camel(2).And.Gold(3).And.Pepper(2), Reward.Of.Coin(7).And.Vp(8)))
                .SetAvailableDice(6, 6, 6, 6, 6)
                .GenerateResources(Cost.Of.Coin(7))
                .VerifyResources(new ResourceBag {Coin = 7, Vp = 8});
        }

        [Test]
        public void GenerateResourcesHolistically()
        {
            new TestScenario()
                .ConfigureLargeCity(Alexandria, new ExchangeCityCard(Cost.Of.Camel(3), Reward.Of.Gold(1).And.Silk(1).And.Pepper(1)))
                .ConfigureLargeCity(Moscow, new DieRangeCityCard(1, 5, Reward.Of.Gold(1), 6, 6, Reward.Of.Gold(3)))
                .SetResources(Reward.Of.Camel(9))
                .SetContracts(new Contract(Cost.Of.Camel(3).And.Gold(1).And.Silk(1).And.Pepper(1), Reward.Of.Die(1).And.Vp(5)))
                .SetAvailableDice(6, 6, 6, 6, 6)
                .TryToCompleteContracts()
                .VerifyResources(new ResourceBag {Camel = 3, Gold = 2, Silk = 2, Pepper = 2});
        }

        [Test]
        public void MoveUsingCamelToMoveAction()
        {
            new TestScenario()
                .ConfigureLargeCity(Samarcanda, new ExchangeCityCard(Cost.Of.Camel(2), Reward.Of.Move(1)))
                .SetResources(Reward.Of.Camel(9))
                .SetContracts()
                .SetAvailableDice(6, 6, 6, 6, 6)
                .SetLocation(Samarcanda)
                .SetGoals(new Objective(Alexandria,Samarcanda,5), new Objective(Kochi, Samarcanda,5))
                .GenerateResources(Cost.Of.Move(3))
                .VerifyLocation(Alexandria);
        }

        [Test]
        public void UseCityBonusesToGenerateResources()
        {
            var bonusPerCoinCard = new ExchangeCityCard(Cost.Of.Coin(1), Reward.Of.TradingPostBonus(1));
            new TestScenario()
                .BuildTradingPostForCityBonus(r => r.Coin == 5)
                .BuildTradingPostForCityBonus(r => r.Camel == 1 && r.Coin == 3)
                .BuildTradingPostForCityBonus(r => r.OtherCityBonus == 1)
                .ConfigureLargeCity(Alexandria, bonusPerCoinCard)
                .SetAvailableDice(1, 4, 6)
                .SetResources(Reward.Of.Coin(3))
                .SetContracts()
                .SetNextMove(new Route(Alexandria, Kochi, Cost.Of.Coin(10)))
                // AI should choose to play on Alexandria space and pay 3 coins to generate trading post bonuses for a total of 13 coins and 1 camel
                .GenerateResources(Cost.Of.Coin(10))
                .VerifyResources(new ResourceBag {Coin = 13, Camel = 1});
        }
    }
}
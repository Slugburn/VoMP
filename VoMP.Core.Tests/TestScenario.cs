using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using VoMP.Core.Actions;
using VoMP.Core.Behavior;
using VoMP.Core.Characters;
using VoMP.Core.CityCards;
using VoMP.Core.Extensions;

namespace VoMP.Core.Tests
{
    public class TestScenario
    {
        private readonly Game _game;
        private readonly Player _player;
        private Route[] _nextMove;

        public TestScenario()
        {
            _game = new Game();
            var characters = Enumerable.Range(1, 4).Select(x => new TestCharacter(x));
            _game.SetUp(characters);
            // clear all outpost bonuses to prevent random bonuses from changing expected outcomes
            _game.GetMapLocations().ForEach(l => l.OutpostBonus = null);
            _player = _game.StartPlayer;
            _player.AvailableDice = new List<Die>();
            _player.Behavior = new AiBehavior {State = new AiState(_player, AiBehavior.GetBestPath(_player))};

            _nextMove = new Route[0];
        }

        internal TestScenario AddPawn()
        {
            _player.PawnCount++;
            _player.MovePawn(Location.Unknown, _player.StartLocation);
            return this;
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
            var state = new AiState(_player, AiBehavior.GetBestPath(_player));
            state.NextMove.AddRange(_nextMove);
            ((AiBehavior) _player.Behavior).State = state;
            return state;
        }

        public TestScenario SetGoals(Objective objective1, Objective objective2)
        {
            _player.Objectives = new List<Objective> {objective1, objective2};
            return this;
        }

        public TestScenario SetLocation(Location location)
        {
            _player.MovePawn(Location.Venezia, location);
            return this;
        }

        public TestScenario VerifyPawnLocations(params Location[] locations)
        {
            Assert.That(_player.GetPawnLocations(), Is.EquivalentTo(locations));
            return this;
        }

        public TestScenario Move(params Route[] path)
        {
            _player.Move(path);
            return this;
        }

        public TestScenario VerifyTradingPosts(params Location[] locations)
        {
            Assert.That(_player.TradingPosts, Is.EquivalentTo(locations));
            return this;
        }
    }
}
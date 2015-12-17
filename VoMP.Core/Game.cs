using System.Collections.Generic;
using System.Linq;
using VoMP.Core.CityCards;

namespace VoMP.Core
{
    public class Game
    {
        private Dictionary<Location, CityBonus> _cityBonuses;
        private Dictionary<Location, OutpostBonus> _outpostBonuses;
        private ILookup<Location, ICityCard> _cityCards;
        private List<Contract> _contracts;
        private List<Player> _players;

        public void SetUp()
        {
            // Assign random city bonuses to small cities
            _cityBonuses = Locations.SmallCities.PairWithRandom(CityBonus.CreateAll()).ToDictionary(x => x.Item1, x => x.Item2);
            
            // Assign random outpost bonuses to large cities
            _outpostBonuses = Locations.LargeCities.PairWithRandom(OutpostBonus.CreateAll()).ToDictionary(x=>x.Item1,x=>x.Item2);

            // Assign random city cards to large cities (3 to Sumatra)
            var cityCardLocations = Locations.LargeCities.Union(new[] {Location.Sumatra, Location.Sumatra}).ToList();
            _cityCards = cityCardLocations.PairWithRandom(CityCard.CreateAll()).ToLookup(x => x.Item1, x => x.Item2);

            // Shuffle contracts
            _contracts = Contract.CreateContracts().Shuffle();

            // Create players
            _players = new[] {Color.Blue, Color.Green, Color.Red, Color.Yellow}.Select(c => new Player(c)).Shuffle();
            // Give each player a starting contract
            _players.PairWithRandom(Contract.CreateStartingContracts()).ForEach(x=>x.Item1.GainContract(x.Item2));
            // Grant starting resources
            _players.ForEach(p =>
            {
                p.Camels = 2;
                p.Coins = 7 + _players.IndexOf(p);
            });
        }

        public void StartRound()
        {
            
        }
    }
}

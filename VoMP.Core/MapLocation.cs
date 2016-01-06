using System.Collections.Generic;
using System.Linq;
using VoMP.Core.Actions;
using VoMP.Core.Extensions;

namespace VoMP.Core
{
    public class MapLocation 
    {
        public MapLocation(Location location)
        {
            Location = location;
        }

        public Location Location { get; }
        public CityBonus CityBonus { get; set; }
        public OutpostBonus OutpostBonus { get; set; }
        public List<LargeCityAction> Actions { get; } = new List<LargeCityAction>();
        public List<Color> Pawns { get; } = new List<Color>();
        public List<Color> TradingPosts { get; } = new List<Color>();

        public override string ToString()
        {
            if (IsEmpty()) return null;
            var pawns = Pawns.Select(p => $"@{p}").ToDelimitedString(";");
            var tradingPosts = TradingPosts.Select(tp => $"^{tp}").ToDelimitedString(";");
            var cityCards = Actions.Select(a => $"[{a.Card}]").ToDelimitedString(";");
            string outpostBonus = OutpostBonus != null ? $"*{OutpostBonus}* " : null;
            return $"{Location}: {pawns} {tradingPosts} {CityBonus}{outpostBonus}{cityCards}";
        }

        private bool IsEmpty()
        {
            return CityBonus == null && OutpostBonus == null && !Actions.Any() && !Pawns.Any() &&!TradingPosts.Any();
        }

        public void AddPawn(Color color)
        {
            Pawns.Add(color);
        }

        public void GrantCityBonusTo(Player player)
        {
            var reward = CityBonus.Reward;
            player.GainReward(reward, $"{Location} trading post bonus");
        }

        public void RemovePawn(Color color)
        {
            Pawns.Remove(color);
        }

        public void ClaimOutpostBonus(Player player)
        {
            player.GainReward(OutpostBonus.Reward, $"building first trading post in {Location}");
            OutpostBonus = null;
        }
    }
}
using System.Linq;
using VoMP.Core.CityCards;

namespace VoMP.Core.Actions
{
    public class LargeCityAction : SpaceAction
    {
        public ICityCard Card { get; }
        private readonly Location _location;

        public LargeCityAction(Location location, ICityCard cityCard) : base($"{location} : {cityCard}", 1)
        {
            _location = location;
            Card = cityCard;
        }

        public void Execute(Player player)
        {
            throw new System.NotImplementedException();
        }

        public override bool IsValid(Player player)
        {
            return base.IsValid(player)
                   && !Dice.Any()
                   && player.HasTradingPost(_location)
                   && (Card.OptimumValue(player) > 0 || ((Card as OptionCityCard)?.MaxValue(player,1) > 0));
        }
    }
}
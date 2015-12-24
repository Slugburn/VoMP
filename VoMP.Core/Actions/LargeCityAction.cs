using System.Linq;
using VoMP.Core.CityCards;

namespace VoMP.Core.Actions
{
    public class LargeCityAction : ActionSpace
    {
        public ICityCard Card { get; }
        private readonly Location _location;

        public LargeCityAction(Location location, ICityCard cityCard) : base($"{location} : {cityCard}", 1)
        {
            _location = location;
            Card = cityCard;
        }
    }
}
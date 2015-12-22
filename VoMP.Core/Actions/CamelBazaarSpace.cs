namespace VoMP.Core.Actions
{
    public class CamelBazaarSpace : GrandBazaarSpace
    {
        public CamelBazaarSpace() : base("Grand Bazaar (Camels)", 1)
        {
        }

        public override Reward GetReward(int value)
        {
            return new Reward { Camel = value };
        }
    }
}
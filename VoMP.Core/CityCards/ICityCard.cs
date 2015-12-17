namespace VoMP.Core.CityCards
{
    public interface ICityCard
    {
        bool IsReversible { get; }

        int MaxValue(Player player, bool reversed);

        Cost GetCost(int dieValue, bool reversed);

        Reward GetReward(int dieValue, bool reversed);
    }
}
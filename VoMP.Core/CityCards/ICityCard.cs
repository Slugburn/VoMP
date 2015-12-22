namespace VoMP.Core.CityCards
{
    public interface ICityCard
    {
        Cost GetCost(int dieValue);

        Reward GetReward(Player player, int dieValue);

        bool CanGenerate(Player player, ResourceType resourceType);

        int OptimumValue(Player player);
    }
}
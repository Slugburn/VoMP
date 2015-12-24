namespace VoMP.Core.Behavior.Choices
{
    public interface ISpaceActionChoice : IActionChoice
    {
        Cost GetCost();
        Reward GetReward();
    }
}

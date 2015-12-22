namespace VoMP.Core.Actions
{
    public interface IAction
    {
        bool IsValid(Player player);
        string Description { get;  }
    }
}
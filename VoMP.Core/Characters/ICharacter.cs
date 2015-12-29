namespace VoMP.Core.Characters
{
    public interface ICharacter
    {
        string Name { get; }

        void ModifyPlayer(Player player);
    }
}

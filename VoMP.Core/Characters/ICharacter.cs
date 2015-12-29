namespace VoMP.Core.Characters
{
    public interface ICharacter
    {
        string Name { get; }

        void Claim(Player player);
    }
}

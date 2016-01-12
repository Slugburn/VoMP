using VoMP.Core.Characters;

namespace VoMP.Core.Tests
{
    class TestCharacter : ICharacter
    {
        private readonly int _id;

        public TestCharacter(int id)
        {
            _id = id;
        }

        public string Name => $"Test #{_id}";

        public void ModifyPlayer(Player player)
        {
            // do nothing
        }
    }
}

using System;

namespace VoMP.Core.Characters
{
    public class Johannes : ICharacter
    {
        public string Name => "Johannes Caprini";
        public void ModifyPlayer(Player player)
        {
            // All oasises are adjacent to each other, bonus 3 coins
            throw new NotImplementedException();
        }

    }
}

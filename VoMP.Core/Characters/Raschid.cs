using System;

namespace VoMP.Core.Characters
{
    public class Raschid : ICharacter
    {
        public string Name => "Raschid ad-Din Sinan";
        public void ModifyPlayer(Player player)
        {
            // Assigns dice values
            player.RollsDice = false;
        }

    }
}

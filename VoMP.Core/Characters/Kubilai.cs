using System;

namespace VoMP.Core.Characters
{
    public class Kubilai : ICharacter
    {
        public string Name => "Kubilai Khan";
        public void ModifyPlayer(Player player)
        {
            // Starts in Beijing
            player.MovePawn(Location.Unknown, Location.Beijing);
            player.BuildTradingPost(Location.Beijing);
        }

    }
}

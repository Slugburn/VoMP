using System;

namespace VoMP.Core.Characters
{
    public class Johannes : ICharacter
    {
        public string Name => "Johannes Caprini";
        public void ModifyPlayer(Player player)
        {
            // All oasises are adjacent to each other
            player.RouteMap = RouteMap.Oasis();
            // Bonus 3 coins
            player.CharacterBonuses.Add(p=>p.GainReward(Reward.Of.Coin(3), Name ));
        }

    }
}

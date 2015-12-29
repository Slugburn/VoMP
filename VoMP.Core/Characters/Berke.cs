namespace VoMP.Core.Characters
{
    public class Berke : ICharacter
    {
        public string Name => "Berke Khan";
        public void ModifyPlayer(Player player)
        {
            // Use occupied action space without paying coin
            player.GetOccupancyCost = (space, dice, value) => Cost.None;
        }

    }
}

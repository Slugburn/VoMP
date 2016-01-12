namespace VoMP.Core.Characters
{
    public class NicoloAndMarco : ICharacter
    {
        public string Name => "Nicolo and Marco Polo";
        public void ModifyPlayer(Player player)
        {
            // Two pawns, bonus camel
            player.PawnCount = 2;
            player.CharacterBonuses.Add(BonusCamel);

        }

        private void BonusCamel(Player player)
        {
            player.GainReward(Reward.Of.Camel(1), Name);
        }
    }
}

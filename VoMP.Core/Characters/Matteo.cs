namespace VoMP.Core.Characters
{
    public class Matteo : ICharacter
    {
        public string Name => "Matteo Polo";

        public void ModifyPlayer(Player player)
        {
            // Bonus white die and contract
            player.CharacterBonuses.Add(BonusWhiteDie);
            player.CharacterBonuses.Add(BonusContract);
        }

        private static void BonusWhiteDie(Player player)
        {
            player.Output("gains bonus white die");
            player.GainDie(Color.White);
        }

        private void BonusContract(Player player)
        {
            player.GainReward(Reward.Of.Contract(1), Name);
        }

    }
}

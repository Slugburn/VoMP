using System;
using VoMP.Core.Behavior.Choices;
using VoMP.Core.Behavior.Choices.Bazaar;

namespace VoMP.Core.Characters
{
    public class Mercator : ICharacter
    {
        public string Name => "Mercator ex Tabriz";
        public void ModifyPlayer(Player player)
        {
            // Free resources when other character visits marketplace
            player.NotifyOfActionChoice = OnOtherPlayerAction;
        }

        public void OnOtherPlayerAction(Player player, IActionChoice choice)
        {
            if (choice is GoldBazaar)
                player.GainReward(new Reward {Gold = 1}, Name);
            if (choice is SilkBazaar)
                player.GainReward(new Reward {Silk = 1}, Name);
            if (choice is PepperBazaar)
                player.GainReward(new Reward {Pepper = 1}, Name);
            if (choice is CamelBazaar)
                player.GainReward(new Reward {Camel = 1}, Name);
        }

    }
}

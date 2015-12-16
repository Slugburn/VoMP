using System;

namespace VoMP.Core
{
    public class Player
    {
        public int Camels { get; set; }
        public int Silk { get; set; }
        public int Coins { get; set; }
        public int Gold { get; set; }

        public void GainGood(int count)
        {
            throw new NotImplementedException();
        }

        public void GainBonusContract()
        {
            throw new NotImplementedException();
        }

        public void GainBlackDie()
        {
            throw new NotImplementedException();
        }

        public void TakeBonusMove()
        {
            throw new NotImplementedException();
        }
    }
}
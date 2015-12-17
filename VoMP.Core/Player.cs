using System;
using System.Collections.Generic;

namespace VoMP.Core
{
    public class Player
    {
        public Color Color { get; set; }

        public Player(Color color)
        {
            Color = color;
        }

        public int Camels { get; set; }
        public int Silk { get; set; }
        public int Coins { get; set; }
        public int Gold { get; set; }
        public List<Contract> CompletedContracts { get; set; }
        public List<Location> TradingPosts { get; set; }

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

        public bool CanPay(Cost cost)
        {
            throw new NotImplementedException();
        }

        public void GainContract(Contract contract)
        {
            throw new NotImplementedException();
        }
    }
}
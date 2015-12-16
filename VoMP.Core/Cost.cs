using System;
using System.Collections.Generic;

namespace VoMP.Core
{
    public class Cost
    {
        public int Camel { get; set; }
        public int Gold { get; set; }
        public int Silk { get; set; }
        public int Pepper { get; set; }

        public override string ToString()
        {
            return string.Join(",", GetDescriptions());
        }

        private IEnumerable<string> GetDescriptions()
        {
            if (Camel > 0)
                yield return Camel == 1 ? "Camel" : $"{Camel} Camels";
            if (Gold > 0)
                yield return Gold == 1 ? "Gold" : $"{Gold} Gold";
            if (Silk > 0)
                yield return Silk == 1 ? "Silk" : $"{Silk} Silk";
            if (Pepper > 0)
                yield return Pepper == 1 ? "Pepper" : $"{Pepper} Pepper";
        }
    }
}
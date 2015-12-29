using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoMP.Core.Characters
{
    public class Mercator : ICharacter
    {
        public string Name => "Mercator ex Tabriz";
        public void Claim(Player player)
        {
            // Free resources when other character visits marketplace
            throw new NotImplementedException();
        }

    }
}

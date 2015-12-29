using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoMP.Core.Characters
{
    public class Kubilai : ICharacter
    {
        public string Name => "Kubilai Khan";
        public void ModifyPlayer(Player player)
        {
            // Starts in Beijing
            throw new NotImplementedException();
        }

    }
}

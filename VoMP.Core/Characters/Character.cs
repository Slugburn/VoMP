using System.Collections.Generic;

namespace VoMP.Core.Characters
{
    public static class Character
    {
        public static IList<ICharacter> CreateBasic() => new ICharacter[] {new Raschid(), new Matteo(), new Berke(), new Kubilai(),  new Mercator()};

        public static IEnumerable<ICharacter> CreateAll() =>
            new ICharacter[]
            {
                new Berke(), new Johannes(), new Kubilai(), new Matteo(), new Mercator(), new NicoloAndMarco(), new Raschid(), new Wilhelm()
            };
    }
}
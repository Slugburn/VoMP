using System;

namespace VoMP.Core.Behavior.Choices.Bazaar
{
    public static class Bazaar
    {
        public static T Create<T>(Player player) where T : BazaarBase
        {
            var constructor = typeof (T).GetConstructor(new [] { typeof(Player) });
            if (constructor==null)
                throw new InvalidOperationException();
            return (T) constructor.Invoke(new object[] {player});
        }
    }
}

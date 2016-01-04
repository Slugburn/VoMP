using System.Collections.Generic;
using VoMP.Core.Actions;

namespace VoMP.Core.Behavior.Choices
{
    public interface ISpaceActionChoice : IActionChoice
    {
        IList<Die> Dice { get; }
        int Value { get;}
        ActionSpace Space { get; }
    }
}

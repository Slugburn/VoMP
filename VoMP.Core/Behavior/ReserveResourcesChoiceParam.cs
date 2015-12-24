using System;
using System.Collections.Generic;
using VoMP.Core.Behavior.Choices;

namespace VoMP.Core.Behavior
{
    public class ReserveResourcesChoiceParam
    {
        public ReserveResourcesChoiceParam(Func<IActionChoice> makeChoice, string reason)
        {
            MakeChoice = makeChoice;
            Reason = reason;
        }

        public Func<IActionChoice> MakeChoice { get; }
        public Cost Cost { get; set; }
        public string Reason { get; }
        public IEnumerable<Die> Dice { get; set; }
    }
}
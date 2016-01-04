using System;
using System.Collections.Generic;
using System.Linq;
using VoMP.Core.Actions;
using VoMP.Core.Behavior.Choices;

namespace VoMP.Core.Extensions
{
    public static class ActionExtension
    {
        public static T Get<T>(this IEnumerable<IAction> actions) where T:IAction
        {
            return actions.OfType<T>().SingleOrDefault() ;
        }

        public static int Rating(this ISpaceActionChoice choice)
        {
            return choice.Reward.Rating - choice.Cost.Rating - choice.Space.RequiredDice*3;
        }
    }
}

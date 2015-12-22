using System;
using System.Collections.Generic;
using System.Linq;
using VoMP.Core.Actions;

namespace VoMP.Core.Extensions
{
    public static class ActionExtension
    {
        public static T Get<T>(this IEnumerable<IAction> actions) where T:IAction
        {
            return actions.OfType<T>().SingleOrDefault() ;
        }
    }
}

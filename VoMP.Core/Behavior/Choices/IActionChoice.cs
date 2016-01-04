using System.Collections.Generic;

namespace VoMP.Core.Behavior.Choices
{
    public interface IActionChoice : IExchange
    {
        void Execute();
        bool IsValid();
    }
}

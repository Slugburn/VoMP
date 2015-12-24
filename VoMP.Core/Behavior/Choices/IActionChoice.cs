﻿using System.Collections.Generic;

namespace VoMP.Core.Behavior.Choices
{
    public interface IActionChoice
    {
        void Execute();
        bool IsValid();
    }
}

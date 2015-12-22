using System;

namespace VoMP.Core
{
    internal class ConsoleOutput : IOutput
    {
        public void Write(object o)
        {
            Console.WriteLine(o);
        }
    }
}
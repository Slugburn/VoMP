using System;

namespace VoMP.Core
{
    public class UnassignedDieException : ApplicationException
    {
        public UnassignedDieException() : base("Unable to process die with unassigned value")
        {
        }
    }
}
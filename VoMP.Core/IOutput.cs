namespace VoMP.Core
{
    internal interface IOutput
    {
        void Write(object o);
        void WriteDebug(object o);
    }
}
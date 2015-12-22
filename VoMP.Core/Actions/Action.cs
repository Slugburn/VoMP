namespace VoMP.Core.Actions
{
    public abstract class Action : IAction
    {
        public string Description { get;  }

        protected Action(string description)
        {
            Description = description;
        }

        public abstract bool IsValid(Player player);

        public override string ToString()
        {
            return Description;
        }
    }
}
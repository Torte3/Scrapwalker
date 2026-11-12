namespace Scrapwalker.Core.Root
{
    public abstract class Command
    {
        internal abstract CommandResult Check(World world);

        protected abstract void Run(World world);

        internal virtual bool IsAllowedBy(CommandRule rule, World world) => rule.AllowsOther(world, this);

        internal virtual string LogText => GetType().Name;

        internal CommandResult Apply(World world)
        {
            CommandResult check = Check(world);
            if (check.IsOK)
            {
                Run(world);
            }

            return check;
        }
    }
}

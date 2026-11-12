namespace Scrapwalker.Core.Root
{
    public readonly struct CommandPreview
    {
        public CommandPreview(CommandResult rule, CommandResult own)
        {
            Rule = rule;
            Own = own;
        }

        public CommandResult Rule { get; }
        public CommandResult Own { get; }
        public bool IsOK => Rule.IsOK && Own.IsOK;
    }
}

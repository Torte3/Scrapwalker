using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Combat
{
    public sealed class ReleaseLockOn : Command
    {
        internal override bool IsAllowedBy(CommandRule rule, World world) => rule.AllowsLockOn(world, this);

        internal override CommandResult Check(World world)
        {
            return world.Battle.LockedOn != null ? CommandResult.Success() : CommandResult.Fail(CommandError.NoLockOn);
        }

        protected override void Run(World world)
        {
            world.Battle.SetLockOn(null);
        }
    }
}

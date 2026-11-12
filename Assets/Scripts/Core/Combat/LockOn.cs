using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Combat
{
    public sealed class LockOn : Command
    {
        public LockOn(int enemyID)
        {
            EnemyID = enemyID;
        }

        public int EnemyID { get; }

        internal override bool IsAllowedBy(CommandRule rule, World world) => rule.AllowsLockOn(world, this);

        internal override CommandResult Check(World world)
        {
            Enemy enemy = world.Battle.FindEnemy(EnemyID);
            return enemy != null && enemy.IsAlive ? CommandResult.Success() : CommandResult.Fail(CommandError.NoSuchEnemy);
        }

        protected override void Run(World world)
        {
            world.Battle.SetLockOn(world.Battle.FindEnemy(EnemyID));
        }
    }
}

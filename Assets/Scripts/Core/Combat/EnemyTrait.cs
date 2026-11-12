using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Combat
{
    public abstract class EnemyTrait
    {
        public virtual bool AcceptsHeal => true;

        public abstract void Show(IEnemyTraitView view);

        internal virtual void Arrive()
        {
        }

        internal virtual void Recover(GameConfiguration configuration)
        {
        }

        internal virtual bool TryBlock(GameConfiguration configuration) => false;

        internal virtual void Act(World world, Enemy enemy)
        {
        }
    }
}

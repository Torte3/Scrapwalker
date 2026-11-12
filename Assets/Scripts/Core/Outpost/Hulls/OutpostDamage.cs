using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Hulls
{
    public static class OutpostDamage
    {
        internal static int Take(World world, int damage, Cell hit)
        {
            int reached = world.Barrier.Absorb(damage);
            world.Facilities.At(hit)?.TakeDamage(world, reached);
            DamageHull(world, reached, hit);
            return reached;
        }

        private static void DamageHull(World world, int amount, Cell hit)
        {
            if (world.Hull.TakeHit(amount, hit))
            {
                world.Emit(new HullBreached());
            }
        }
    }
}

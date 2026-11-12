using Scrapwalker.Core.Combat;

namespace Scrapwalker.Game.MainView.Enemies
{
    internal sealed class EnemyTraitReader : IEnemyTraitView
    {
        public float HealReach { get; private set; }
        public int BarrierLayers { get; private set; }
        public int BarrierMaxLayers { get; private set; }
        public int BarrierBrokenCount { get; private set; }

        public EnemyTraitReader Read(Enemy enemy)
        {
            HealReach = 0f;
            BarrierLayers = 0;
            BarrierMaxLayers = 0;
            BarrierBrokenCount = 0;
            foreach (EnemyTrait trait in enemy.Traits)
            {
                trait.Show(this);
            }

            return this;
        }

        public void Healing(float reach)
        {
            HealReach = reach;
        }

        public void Barrier(int layers, int maxLayers, int brokenCount)
        {
            BarrierLayers = layers;
            BarrierMaxLayers = maxLayers;
            BarrierBrokenCount = brokenCount;
        }
    }
}

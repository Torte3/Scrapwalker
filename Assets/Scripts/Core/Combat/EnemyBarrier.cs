using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Combat
{
    public sealed class EnemyBarrier : EnemyTrait
    {
        private readonly Countdown _recovery = new Countdown();
        private readonly BarrierAbility _ability;

        internal EnemyBarrier(BarrierAbility ability)
        {
            _ability = ability;
            Layers = ability.Layers;
        }

        public int MaxLayers => _ability.Layers;
        public int Layers { get; private set; }
        public int BrokenCount { get; private set; }

        public override void Show(IEnemyTraitView view)
        {
            view.Barrier(Layers, MaxLayers, BrokenCount);
        }

        internal override bool TryBlock(GameConfiguration configuration)
        {
            if (Layers == 0)
            {
                return false;
            }

            if (Layers == MaxLayers)
            {
                _recovery.Start(RecoverTicks(configuration));
            }

            Layers--;
            BrokenCount++;
            return true;
        }

        internal override void Recover(GameConfiguration configuration)
        {
            if (Layers == MaxLayers)
            {
                return;
            }

            if (!_recovery.CountDownOne())
            {
                return;
            }

            Layers++;
            _recovery.Start(RecoverTicks(configuration));
        }

        private int RecoverTicks(GameConfiguration configuration)
        {
            return configuration.TicksAtLeastOne(_ability.RecoverSeconds);
        }
    }
}

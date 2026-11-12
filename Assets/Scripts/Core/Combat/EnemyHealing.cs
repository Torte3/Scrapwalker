using System.Collections.Generic;
using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Combat
{
    public sealed class EnemyHealing : EnemyTrait
    {
        private readonly HealAbility _ability;
        private readonly Countdown _wait = new Countdown();

        internal EnemyHealing(HealAbility ability)
        {
            _ability = ability;
        }

        public override bool AcceptsHeal => false;

        public override void Show(IEnemyTraitView view)
        {
            view.Healing(_ability.Reach);
        }

        internal override void Arrive()
        {
            _wait.End();
        }

        internal override void Act(World world, Enemy healer)
        {
            bool isDue = _wait.IsDone;
            if (isDue)
            {
                _wait.Extend(world.Configuration.TicksAtLeastOne(_ability.Seconds));
            }

            _wait.Advance(healer.Pace);
            if (!isDue)
            {
                return;
            }

            var healed = new List<EnemyHeal>();
            foreach (Enemy enemy in world.Battle.Enemies)
            {
                if (!enemy.IsAlive || !enemy.AcceptsHeal || System.Math.Abs(enemy.Distance - healer.Distance) > _ability.Reach)
                {
                    continue;
                }

                float amount = enemy.Heal(_ability.Amount);
                if (amount > 0f)
                {
                    healed.Add(new EnemyHeal(enemy.ID, amount));
                }
            }

            world.Emit(new EnemiesHealed(healer, _ability.Reach, healed));
        }
    }
}

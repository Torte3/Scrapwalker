using System;
using System.Collections.Generic;
using System.Linq;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Combat
{
    public sealed class Enemy
    {
        public int ID { get; }
        public EnemyDefinition Definition { get; }
        public float Distance { get; internal set; }
        public float HP { get; internal set; }
        public float MaxHP { get; }
        public IReadOnlyList<MaterialAmount> Drops { get; internal set; } = Array.Empty<MaterialAmount>();
        public bool HasBegunAttacking { get; internal set; }
        public int StrikeCount { get; internal set; }
        public AttackRoutine Attack { get; } = new AttackRoutine();
        public IReadOnlyList<EnemyTrait> Traits { get; }
        public int SlowTicks { get; internal set; }
        public float SlowFactor { get; internal set; } = 1f;
        public float Pace => PaceWith(SlowTicks);

        public float AttackPeriodSeconds => (Definition.HasAttacks ? Definition.AttackMoves[Attack.Index].RestSeconds : 1f) / Pace;

        public bool IsAlive => HP > 0f;
        public bool IsInPosition => Distance <= StopDistance;
        public bool AcceptsHeal => Traits.All(trait => trait.AcceptsHeal);
        private float StopDistance => Definition.Type.StopDistance;

        public Enemy(int id, EnemyDefinition definition, float distance, float hp)
        {
            ID = id;
            Definition = definition;
            Distance = distance;
            HP = hp;
            MaxHP = hp;
            Traits = definition.Type.Abilities.Select(ability => ability.Start()).ToList();
        }

        internal void Walk(float deltaTime)
        {
            int slow = SlowTicks;
            Distance = Walked(Distance, ref slow, deltaTime);
            SlowTicks = slow;
        }

        public float DistanceAfter(int ticks, float deltaTime)
        {
            float distance = Distance;
            int slow = SlowTicks;
            for (int i = 0; i < ticks && distance > StopDistance; i++)
            {
                distance = Walked(distance, ref slow, deltaTime);
            }

            return distance > StopDistance ? distance : Math.Max(0f, StopDistance);
        }

        public float DamageThroughArmor(float damage, float armorPiercing)
        {
            return Math.Max(0f, damage - Math.Max(0f, Definition.Armor - armorPiercing));
        }

        internal bool TryBlockHit(GameConfiguration configuration)
        {
            return Traits.Any(trait => trait.TryBlock(configuration));
        }

        internal void Recover(GameConfiguration configuration)
        {
            foreach (EnemyTrait trait in Traits)
            {
                trait.Recover(configuration);
            }
        }

        internal void Arrive()
        {
            foreach (EnemyTrait trait in Traits)
            {
                trait.Arrive();
            }
        }

        internal void Act(World world)
        {
            foreach (EnemyTrait trait in Traits)
            {
                trait.Act(world, this);
            }
        }

        internal void SlowDown(int ticks, float factor)
        {
            SlowTicks = Math.Max(SlowTicks, ticks);
            SlowFactor = factor;
        }

        internal float Heal(float amount)
        {
            float healed = Math.Min(amount, MaxHP - HP);
            HP += healed;
            return healed;
        }

        internal void Knockback(float push)
        {
            Distance += push;
            HasBegunAttacking = false;
            Attack.Restart(0, 0);
        }

        private float Walked(float distance, ref int slowTicks, float deltaTime)
        {
            if (slowTicks > 0)
            {
                slowTicks--;
            }

            float speed = Definition.Type.Speed * PaceWith(slowTicks);
            return distance > StopDistance ? distance - speed * deltaTime : distance;
        }

        private float PaceWith(int slowTicks)
        {
            return slowTicks > 0 ? SlowFactor : 1f;
        }
    }
}

using System;
using Scrapwalker.Core.Combat;
using Scrapwalker.Core.Combat.Weapons;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost.Bots;
using Scrapwalker.Core.Outpost.Roles;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Facilities.Recipes.Results
{
    [Serializable]
    public sealed class ShotResult : RecipeResult
    {
        public ShotDefinition Shot = new ShotDefinition();

        public override WorkPriority Priority => WorkPriority.Attack;

        public override T ShowPose<T>(IWorkPoseView<T> view)
        {
            return view.Firing();
        }

        internal override bool UsesUpInputs => true;

        private int RoundCount => Math.Max(1, Shot.Rounds);

        public override void Check(FacilityDefinition facility, RecipeDefinition recipe, IRecipeCheck check)
        {
            if (recipe.When != TaskWindow.InCombat || (!Shot.Strikes && !Shot.Slow.IsActive))
            {
                check.BadRecipe();
            }

            if (facility.Muzzle == null || facility.Muzzle.ShotCellsPerSecond <= 0f)
            {
                check.BadWeapon();
            }
        }

        public override void Describe(World world, Facility facility, RecipeDefinition recipe, IRecipeEffects effects)
        {
            if (Shot.Strikes)
            {
                DescribeHit(world, facility, effects);
            }

            if (Shot.Slow.IsActive)
            {
                effects.Slow(Shot.Slow, Shot.Radius);
            }
        }

        private void DescribeHit(World world, Facility facility, IRecipeEffects effects)
        {
            effects.Shot(Damage(world, facility, facility.Power), RoundCount);
            if (Shot.MaxTargets == 0)
            {
                effects.Note(EffectNote.ShotArea);
            }
            else if (Shot.MaxTargets > 1)
            {
                effects.ShotPierce(Shot.MaxTargets);
            }

            if (Shot.ArmorPiercing > 0f)
            {
                effects.ShotArmorPiercing(Shot.ArmorPiercing);
            }

            if (Shot.Knockback > 0f)
            {
                effects.Note(EffectNote.ShotKnockback);
            }
        }

        internal override bool DescribeChange(World world, Facility facility, RecipeDefinition recipe, FacilityStat stat, float from, float to, IRecipeEffects effects)
        {
            if (stat != FacilityStat.Power || !Shot.Strikes)
            {
                return false;
            }

            effects.Power(recipe, Damage(world, facility, (int)from), Damage(world, facility, (int)to));
            return true;
        }

        private float Damage(World world, Facility facility, int power)
        {
            return WeaponSystem.Damage(world.Configuration, facility, Shot, Quality.Plain, power);
        }

        internal override TaskStatus? OutputBlock(World world, Facility facility)
        {
            return world.Battle.Target() != null ? (TaskStatus?)null : TaskStatus.NoEnemyInRange;
        }

        internal override TaskStatus? ProgressBlock(World world, Facility facility, WorkJob job)
        {
            return job.Volley.RoundsLeft > 0 ? OutputBlock(world, facility) : (TaskStatus?)null;
        }

        internal override void Begin(World world, Facility facility, WorkJob job)
        {
            job.Volley = new Volley(RoundCount);
        }

        internal override bool Tick(World world, Facility facility, WorkJob job)
        {
            Enemy target = world.Battle.Target();
            if (target == null || !TryReachRound(facility, job))
            {
                return false;
            }

            FireRound(world, facility, job, target);
            return job.Volley.RoundsLeft == 0;
        }

        private static bool TryReachRound(Facility facility, WorkJob job)
        {
            if (IsCharging(job))
            {
                return job.Time.Advance(facility.StaffSpeed);
            }

            if (!job.Time.TryFillTick(facility.StaffSpeed))
            {
                return false;
            }

            if (!job.Volley.RoundTime.IsDone)
            {
                job.Volley.RoundTime.CountDownOne();
                return false;
            }

            return true;
        }

        private static bool IsCharging(WorkJob job)
        {
            return !job.Time.IsDone;
        }

        internal override void Finish(World world, Facility facility, WorkJob job)
        {
        }

        private void FireRound(World world, Facility facility, WorkJob job, Enemy target)
        {
            world.Battle.RequestFire(new FireRequest(facility, Shot, job.InputQuality ?? Quality.Plain, target));
            job.Volley.RoundsLeft--;
            job.Volley.RoundTime.Start(job.Volley.RoundsLeft > 0 ? world.Configuration.TicksFor(Shot.RoundSeconds) : 0);
        }
    }
}

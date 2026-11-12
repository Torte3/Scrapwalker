using System.Collections.Generic;
using System.Linq;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Combat.Weapons
{
    internal static class WeaponSystem
    {
        internal static void Tick(World world)
        {
            foreach (Projectile projectile in world.Battle.Projectiles)
            {
                projectile.Time.CountDownOne();
            }

            foreach (Projectile projectile in world.Battle.Projectiles.Where(p => p.Time.IsDone).ToList())
            {
                world.Battle.RemoveProjectile(projectile);
                Land(world, projectile);
            }

            foreach (FireRequest request in world.Battle.TakeFireRequests())
            {
                Fire(world, request.Gun, request.Shot, request.BulletQuality, request.Target);
            }
        }

        private static void Fire(World world, Facility gun, ShotDefinition shot, Quality bulletQuality, Enemy target)
        {
            GameConfiguration configuration = world.Configuration;
            float damage = Damage(configuration, gun, shot, bulletQuality, gun.Power);
            int ticks = WeaponAiming.FlightTicks(world, gun, target);
            float landing = WeaponAiming.LandingDistance(world, target, ticks);
            var projectile = new Projectile(gun, shot, MuzzleDistance(world, gun), landing, damage, ticks);
            world.Battle.AddProjectile(projectile);
            world.Emit(new ShotFired(gun, target, projectile, ticks * configuration.TickSeconds));
        }

        public static float Damage(GameConfiguration configuration, Facility gun, ShotDefinition shot, Quality bulletQuality, int powerUpgrades)
        {
            float damage = (shot.Damage + shot.DamagePerUpgrade * powerUpgrades)
                * configuration.Row(bulletQuality).BulletMultiplier
                * configuration.Row(gun.BuiltQuality ?? Quality.Plain).GunBonus;
            return damage > 0f ? System.Math.Max(1f, (float)System.Math.Round(damage, System.MidpointRounding.AwayFromZero)) : 0f;
        }

        internal static int FlightTicks(World world, Facility gun, float distance)
        {
            return world.Configuration.TicksAtLeastOne((distance - MuzzleDistance(world, gun)) / gun.Definition.Muzzle.ShotCellsPerSecond);
        }

        internal static float MuzzleDistance(World world, Facility gun)
        {
            GameConfiguration configuration = world.Configuration;
            float muzzleColumn = gun.Cell.Column + gun.Definition.Width * gun.Definition.Muzzle.ShareOfWidth;
            return -((world.Columns - muzzleColumn) * configuration.CellDistance + configuration.ArmorDistance);
        }

        private static void Land(World world, Projectile projectile)
        {
            Enemy lockedOn = world.Battle.LockedOn;
            ShotDefinition shot = projectile.Shot;
            List<Enemy> reached = world.Battle.Enemies.Where(e => e.IsAlive && shot.Covers(projectile.LandingDistance, e.Distance))
                .OrderBy(e => e == lockedOn ? 0 : 1).ThenBy(e => System.Math.Abs(e.Distance - projectile.LandingDistance)).ToList();
            if (shot.MaxTargets > 0 && shot.MaxTargets < reached.Count)
            {
                reached.RemoveRange(shot.MaxTargets, reached.Count - shot.MaxTargets);
            }

            var impacts = new List<EnemyImpact>();
            foreach (Enemy enemy in reached)
            {
                if (shot.Strikes)
                {
                    impacts.Add(enemy.TryBlockHit(world.Configuration) ? new EnemyImpact(enemy.ID, 0f, 0f, true) : Strike(enemy, projectile));
                }

                if (shot.Slow.IsActive && enemy.IsAlive)
                {
                    enemy.SlowDown(world.Configuration.TicksFor(shot.Slow.Seconds), shot.Slow.Factor);
                }
            }

            world.Emit(new ShotLanded(projectile, impacts));
        }

        private static EnemyImpact Strike(Enemy enemy, Projectile projectile)
        {
            ShotDefinition shot = projectile.Shot;
            float damage = enemy.DamageThroughArmor(projectile.Damage, shot.ArmorPiercing);
            enemy.HP -= damage;
            if (shot.Knockback > 0f && enemy.IsAlive)
            {
                enemy.Knockback(shot.Knockback);
            }

            return new EnemyImpact(enemy.ID, damage, projectile.Damage - damage, false);
        }
    }
}

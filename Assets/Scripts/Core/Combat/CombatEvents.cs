using System.Collections.Generic;
using Scrapwalker.Core.Combat.Weapons;
using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Outpost.Grid;

namespace Scrapwalker.Core.Combat
{
    public sealed class ShotFired : GameEvent
    {
        public ShotFired(Facility gun, Enemy target, Projectile projectile, float flightSeconds)
        {
            FacilityID = gun.ID;
            Weapon = gun.Definition;
            FacilityRect = gun.Rect;
            EnemyID = target.ID;
            Enemy = target.Definition;
            Distance = target.Distance;
            ProjectileID = projectile.ID;
            Origin = projectile.Origin;
            LandingDistance = projectile.LandingDistance;
            Radius = projectile.Shot.Radius;
            FlightSeconds = flightSeconds;
        }

        public int FacilityID { get; }
        public FacilityDefinition Weapon { get; }
        public CellRect FacilityRect { get; }
        public int EnemyID { get; }
        public EnemyDefinition Enemy { get; }
        public float Distance { get; }
        public int ProjectileID { get; }
        public float Origin { get; }
        public float LandingDistance { get; }
        public float Radius { get; }
        public float FlightSeconds { get; }
    }

    public sealed class ShotLanded : GameEvent
    {
        public ShotLanded(Projectile projectile, IReadOnlyList<EnemyImpact> impacts)
        {
            ProjectileID = projectile.ID;
            FacilityID = projectile.Gun.ID;
            Weapon = projectile.Gun.Definition;
            Distance = projectile.LandingDistance;
            Radius = projectile.Shot.Radius;
            Impacts = impacts;
        }

        public IReadOnlyList<EnemyImpact> Impacts { get; }

        public int ProjectileID { get; }
        public int FacilityID { get; }
        public FacilityDefinition Weapon { get; }
        public float Distance { get; }
        public float Radius { get; }
    }

    public sealed class EnemiesHealed : GameEvent
    {
        public EnemiesHealed(Enemy healer, float reach, IReadOnlyList<EnemyHeal> heals)
        {
            HealerID = healer.ID;
            Distance = healer.Distance;
            Reach = reach;
            Heals = heals;
        }

        public int HealerID { get; }
        public float Distance { get; }
        public float Reach { get; }
        public IReadOnlyList<EnemyHeal> Heals { get; }
    }

    public sealed class AttackBegun : GameEvent
    {
        public AttackBegun(Enemy attacker, AttackMove move)
        {
            EnemyID = attacker.ID;
            Enemy = attacker.Definition;
            Style = move.Style;
        }

        public int EnemyID { get; }
        public EnemyDefinition Enemy { get; }
        public AttackStyleDefinition Style { get; }
    }

    public sealed class CellAttacked : GameEvent
    {
        public CellAttacked(Enemy attacker, AttackMove move, Cell cell, int barrierTaken, int reached)
        {
            EnemyID = attacker.ID;
            Enemy = attacker.Definition;
            Style = move.Style;
            Distance = attacker.Distance;
            Cell = cell;
            BarrierTaken = barrierTaken;
            Reached = reached;
        }

        public int EnemyID { get; }
        public EnemyDefinition Enemy { get; }
        public AttackStyleDefinition Style { get; }
        public float Distance { get; }
        public Cell Cell { get; }
        public int BarrierTaken { get; }
        public int Reached { get; }
    }

    public sealed class EnemyKilled : GameEvent
    {
        public EnemyKilled(Enemy enemy)
        {
            EnemyID = enemy.ID;
            Enemy = enemy.Definition;
            Distance = enemy.Distance;
            Drops = enemy.Drops;
        }

        public int EnemyID { get; }
        public EnemyDefinition Enemy { get; }
        public float Distance { get; }
        public IReadOnlyList<MaterialAmount> Drops { get; }
    }
}

using System.Collections.Generic;
using System.Linq;
using Scrapwalker.Core.Combat.Weapons;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Combat
{
    public sealed class Battle
    {
        private readonly World _world;
        private readonly List<Enemy> _enemies = new List<Enemy>();
        private readonly List<Projectile> _projectiles = new List<Projectile>();
        private readonly List<FireRequest> _fireRequests = new List<FireRequest>();
        private int _nextEnemyID = 1;
        private int _nextProjectileID = 1;
        private int _lockedOnID;

        internal Battle(World world)
        {
            _world = world;
        }

        public IReadOnlyList<Enemy> Enemies => _enemies;
        public IReadOnlyList<Projectile> Projectiles => _projectiles;

        public Enemy LockedOn => _lockedOnID != 0 && FindEnemy(_lockedOnID) is Enemy enemy && enemy.IsAlive ? enemy : null;

        internal Enemy AddEnemy(EnemyDefinition definition, float distance, float hp, IReadOnlyList<MaterialAmount> drops)
        {
            var enemy = new Enemy(_nextEnemyID++, definition, distance, hp) { Drops = drops };
            _enemies.Add(enemy);
            return enemy;
        }

        public Enemy FindEnemy(int id)
        {
            return _enemies.Find(e => e.ID == id);
        }

        public Enemy NearestAliveEnemy()
        {
            return _enemies.Where(e => e.IsAlive).OrderBy(e => e.Distance).FirstOrDefault();
        }

        public Enemy Target()
        {
            Enemy lockedOn = LockedOn;
            Enemy target = lockedOn != null && IsInRange(lockedOn) ? lockedOn : NearestAliveEnemy();
            return target != null && IsInRange(target) ? target : null;
        }

        private bool IsInRange(Enemy enemy)
        {
            return enemy.Distance <= _world.Configuration.WeaponRange;
        }

        public int CountAliveEnemies()
        {
            return _enemies.Count(e => e.IsAlive);
        }

        internal void RemoveDeadEnemies()
        {
            _enemies.RemoveAll(e => !e.IsAlive);
            if (LockedOn == null)
            {
                _lockedOnID = 0;
            }
        }

        internal void SetLockOn(Enemy enemy)
        {
            _lockedOnID = enemy?.ID ?? 0;
        }

        internal void Clear()
        {
            _lockedOnID = 0;
            _enemies.Clear();
            _projectiles.Clear();
            _fireRequests.Clear();
        }

        internal void RequestFire(FireRequest request)
        {
            _fireRequests.Add(request);
        }

        internal IReadOnlyList<FireRequest> TakeFireRequests()
        {
            var taken = new List<FireRequest>(_fireRequests);
            _fireRequests.Clear();
            return taken;
        }

        internal void AddProjectile(Projectile projectile)
        {
            projectile.ID = _nextProjectileID++;
            _projectiles.Add(projectile);
        }

        internal void RemoveProjectile(Projectile projectile)
        {
            _projectiles.Remove(projectile);
        }
    }
}

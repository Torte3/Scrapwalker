namespace Scrapwalker.Core.Combat
{
    public readonly struct EnemyImpact
    {
        public EnemyImpact(int enemyID, float damage, float absorbed, bool isBlocked)
        {
            EnemyID = enemyID;
            Damage = damage;
            Absorbed = absorbed;
            IsBlocked = isBlocked;
        }

        public int EnemyID { get; }
        public float Damage { get; }
        public float Absorbed { get; }
        public bool IsBlocked { get; }
    }
}

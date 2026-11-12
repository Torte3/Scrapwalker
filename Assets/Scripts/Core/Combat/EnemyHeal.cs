namespace Scrapwalker.Core.Combat
{
    public readonly struct EnemyHeal
    {
        public EnemyHeal(int enemyID, float amount)
        {
            EnemyID = enemyID;
            Amount = amount;
        }

        public int EnemyID { get; }
        public float Amount { get; }
    }
}

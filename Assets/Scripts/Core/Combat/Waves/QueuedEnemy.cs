namespace Scrapwalker.Core.Combat.Waves
{
    public readonly struct QueuedEnemy
    {
        public QueuedEnemy(EnemyDefinition definition, int atTick)
        {
            Definition = definition;
            AtTick = atTick;
        }

        public EnemyDefinition Definition { get; }
        public int AtTick { get; }
    }
}

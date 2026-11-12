namespace Scrapwalker.Core.Outpost.Facilities
{
    public enum TaskStatus
    {
        Advancing,
        Paused,
        Ready,
        AwaitingBot,
        NoFreeBot,
        Materials,
        HaulingMaterials,
        NoEnergyRoom,
        NotEnoughEnergy,
        NoEnemyInRange,
        NotInCombat,
        BotLimit,
        Upgrading,
        Operating,
        NoTasks,
    }
}

namespace Scrapwalker.Game.Audio
{
    public readonly struct SceneCues
    {
        public SceneCues(bool isArmReaching, bool isArmCarrying, bool isArmDigging, bool hasTouchedDown, bool isLaunchFired, bool isWelding, bool isRepairing)
        {
            IsArmReaching = isArmReaching;
            IsArmCarrying = isArmCarrying;
            IsArmDigging = isArmDigging;
            HasTouchedDown = hasTouchedDown;
            IsLaunchFired = isLaunchFired;
            IsWelding = isWelding;
            IsRepairing = isRepairing;
        }

        public bool IsArmReaching { get; }
        public bool IsArmCarrying { get; }
        public bool IsArmDigging { get; }
        public bool HasTouchedDown { get; }
        public bool IsLaunchFired { get; }
        public bool IsWelding { get; }
        public bool IsRepairing { get; }

        public bool IsRunning(WorkSound sound)
        {
            switch (sound)
            {
                case WorkSound.Construction:
                    return IsWelding;
                case WorkSound.Repair:
                    return IsRepairing;
                case WorkSound.Digging:
                    return IsArmDigging;
                default:
                    throw new System.ArgumentOutOfRangeException(nameof(sound), sound, null);
            }
        }
    }
}

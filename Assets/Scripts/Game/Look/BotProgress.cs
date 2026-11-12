namespace Scrapwalker.Game.Look
{
    public readonly struct BotProgress
    {
        public static readonly BotProgress None = new BotProgress();

        public BotProgress(float value, float ready)
        {
            HasProgress = true;
            Value = value;
            Ready = ready;
        }

        public bool HasProgress { get; }
        public float Value { get; }
        public float Ready { get; }
    }
}

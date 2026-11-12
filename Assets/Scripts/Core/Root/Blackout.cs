using Scrapwalker.Core.Framework;

namespace Scrapwalker.Core.Root
{
    public sealed class Blackout
    {
        private readonly SecondsCountdown _time = new SecondsCountdown();
        private (BlackoutStep Step, float Seconds)[] _steps = System.Array.Empty<(BlackoutStep, float)>();
        private int _index;

        public BlackoutStep Step => _index < _steps.Length ? _steps[_index].Step : BlackoutStep.None;
        public bool IsActive => Step.IsActive;
        public float Progress => IsActive ? _time.Progress : 0f;
        public float Darkness => Step.Darkness(Progress);

        internal void Start(float darken, float hold, float brighten)
        {
            _steps = new[]
            {
                (BlackoutStep.Darkening, darken),
                (BlackoutStep.Dark, hold),
                (BlackoutStep.Brightening, brighten),
            };
            Enter(0);
        }

        internal void Stop()
        {
            Enter(_steps.Length);
        }

        internal void Advance(float deltaTime, System.Action swapScene)
        {
            if (!IsActive || !_time.Advance(deltaTime))
            {
                return;
            }

            bool swaps = Step.SwapsSceneAfter;
            Enter(_index + 1);
            if (swaps)
            {
                swapScene();
            }
        }

        private void Enter(int index)
        {
            _index = index;
            _time.Start(IsActive ? _steps[_index].Seconds : 0f);
        }
    }
}

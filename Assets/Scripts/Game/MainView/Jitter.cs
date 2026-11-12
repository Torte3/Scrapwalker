namespace Scrapwalker.Game.MainView
{
    public sealed class Jitter
    {
        private const int PauseSeed = 7;

        private System.Random _random = new System.Random(11);
        private System.Random _held;

        public float Next()
        {
            return (float)_random.NextDouble();
        }

        public void Frame(bool isPaused)
        {
            if (isPaused)
            {
                _held ??= new System.Random(_random.Next());
                _random = new System.Random(PauseSeed);
            }
            else if (_held != null)
            {
                _random = _held;
                _held = null;
            }
        }
    }
}

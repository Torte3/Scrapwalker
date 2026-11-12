namespace Scrapwalker.Core.Framework
{
    public sealed class GameLog
    {
        private readonly System.Func<int> _tick;

        internal GameLog(System.Func<int> tick)
        {
            _tick = tick;
        }

        public System.Action<string> Sink { get; set; }
        public bool IsEnabled => Sink != null;

        public void Write(System.Func<string> message)
        {
            Sink?.Invoke("[t" + _tick() + "] " + message());
        }
    }
}

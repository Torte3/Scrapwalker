namespace Scrapwalker.Game.MainView.Enemies
{
    public readonly struct EnemyBodyStyle
    {
        public static readonly EnemyBodyStyle Alive = new EnemyBodyStyle(1f, 1f, false, true);
        public static readonly EnemyBodyStyle Fading = new EnemyBodyStyle(1f, 1f, true, true);

        private EnemyBodyStyle(float scale, float opacity, bool isCalm, bool showsStun)
        {
            Scale = scale;
            Opacity = opacity;
            IsCalm = isCalm;
            ShowsStun = showsStun;
        }

        public float Scale { get; }
        public float Opacity { get; }
        public bool IsCalm { get; }
        public bool ShowsStun { get; }

        public static EnemyBodyStyle FarAway(float scale)
        {
            return new EnemyBodyStyle(scale, 0.6f, true, false);
        }
    }
}

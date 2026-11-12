namespace Scrapwalker.Game.Wording.Words
{
    public static class LogWords
    {
        public const string EnemyDrop = "敵の落とし物";
        public const string Got = "を入手";

        public static string Killed(string name)
        {
            return $"{name}を撃破した";
        }

        public static string WaveDig(int number)
        {
            return $"ウェーブ{number}の掘削";
        }
    }
}

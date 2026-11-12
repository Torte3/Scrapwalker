namespace Scrapwalker.Game.Wording.Words
{
    public static class TopBarWords
    {
        public const string Hull = "外壁";
        public const string Barrier = "バリア";
        public const string Power = "蓄電";
        public const string RuinMoving = "移動中";
        public const string RuinGrounded = "接地中の廃墟";
        public const string RuinCleared = "完走";
        public const string WaveNone = "ウェーブ —";
        public const string TimeLeft = "残り";

        public static string HullValue(int hp, int max)
        {
            return $"{hp} / {max}";
        }

        public static string BarrierValue(int now, int max)
        {
            return $"{now} / {max}";
        }

        public static string PowerValue(int now, int max)
        {
            return $"{now} / {max}";
        }

        public static string Wave(int number, int count)
        {
            return $"ウェーブ {number} / {count}";
        }

        public static string WaveHauling(int number, int count)
        {
            return $"ウェーブ {number} / {count}\u3000回収中";
        }

        public static string WaveLeftEarly(int number)
        {
            return $"ウェーブ{number}で切り上げ";
        }

        public static string TimeSeconds(string seconds)
        {
            return $"{seconds}秒";
        }
    }
}

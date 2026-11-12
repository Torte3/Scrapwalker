namespace Scrapwalker.Game.Wording.Words
{
    public static class ResearchWords
    {
        public const string Requires = "前提の研究";
        public const string RequiresNone = "なし";
        public const string Now = "研究中";
        public const string NowNone = "研究中の研究は無い";
        public const string NowWaiting = "別のタスクを待っています";
        public const string Cancel = "研究をやめる";
        public const string DetailNone = "左の研究ツリーから、研究を選んでください。";
        public const string Unlocks = "追加されるもの";
        public const string UnlocksNone = "なし";
        public const string UnlockFacility = "設備";
        public const string UnlockTask = "作業";
        public const string UnlockUpgrade = "改造";

        public static string RequiresDone(string name)
        {
            return $"{name} ✓";
        }

        public static string NowCount(int delivered, int total)
        {
            return $"{delivered} / {total} 運び込み";
        }

        public static string NowWaitingFor(string name)
        {
            return $"{name}を待っています";
        }

        public static string UnlockIn(string name, string facility)
        {
            return $"{name}（{facility}）";
        }
    }
}

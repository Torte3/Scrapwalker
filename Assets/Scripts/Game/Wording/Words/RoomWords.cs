namespace Scrapwalker.Game.Wording.Words
{
    public static class RoomWords
    {
        public const string TabResearch = "研究";
        public const string TabWork = "作業";
        public const string SectionFacility = "設備";
        public const string HP = "体力";
        public const string FireInterval = "撃つ間隔";
        public const string Stationing = "常駐";
        public const string StationPeace = "平常時";
        public const string StationCombat = "戦闘中";
        public const string SectionStore = "中のアイテム";
        public const string EmptyStore = "アイテムはありません";
        public const string Capacity = "容量";
        public const string SectionTasks = "作業";
        public const string EmptyRegister = "登録できるタスクはありません";
        public const string Times = "回数";
        public const string Fewer = "−";
        public const string More = "+";
        public const string Run = "実行する";
        public const string SectionUpgrades = "改造";
        public const string NoUpgrades = "いまできる改造はない";
        public const string SectionDoneUpgrades = "実行した改造";
        public const string NoDoneUpgrades = "まだ無い";
        public const string UpgradeDone = "済み";
        public const string SectionQueue = "実行予定";
        public const string EmptyTasks = "タスクはありません";
        public const string Repeat = "繰り返し";
        public const string Up = "▲";
        public const string Down = "▼";
        public const string Remove = "×";

        public static string Position(int floor, int width, int height)
        {
            return $"{floor}F・{width}×{height}マス";
        }

        public static string HPValue(int hp, int max)
        {
            return $"{hp} / {max}";
        }

        public static string FireIntervalValue(string seconds)
        {
            return $"{seconds} 秒に1発";
        }

        public static string CapacityValue(int count, int capacity)
        {
            return $"{count} / {capacity}";
        }

        public static string RecipeSeconds(string seconds)
        {
            return $"{seconds} 秒";
        }

        public static string RunTimes(int times)
        {
            return $"{times} 回実行する";
        }

        public static string RepeatTimes(int times)
        {
            return $"繰り返し {times} 回実行する";
        }

        public static string YieldHeld(int held)
        {
            return $"手持ち {held}";
        }

        public static string YieldStored(int now, int max)
        {
            return $"蓄電 {now} / {max}";
        }
    }
}

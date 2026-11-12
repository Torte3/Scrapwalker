namespace Scrapwalker.Game.Wording.Words
{
    public static class SideWords
    {
        public const string TabStock = "手持ち";
        public const string TabBots = "作業ボット";
        public const string TabSettings = "設定";
        public const string Heading = "移動中";
        public const string Lost = "行けない";
        public const string PlaceFloor = "床";
        public const string PlaceCrack = "外壁のヒビ";
        public const string VolumeBGM = "BGM";
        public const string VolumeSE = "効果音";

        public static string StoreUsage(int used, int capacity)
        {
            return $"資材庫 {used} / {capacity}";
        }

        public static string Cargo(string name, int count)
        {
            return $"{name} {count}";
        }

        public static string VolumeValue(int percent)
        {
            return $"{percent}%";
        }
    }
}

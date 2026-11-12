using Scrapwalker.Core.Save;

namespace Scrapwalker.Game.Wording
{
    public static class SaveText
    {
        public static string SlotName(this SaveSlot slot)
        {
            int index = slot.Index;
            return slot.IsAuto ? "オートセーブ" : $"スロット {index}";
        }
    }
}

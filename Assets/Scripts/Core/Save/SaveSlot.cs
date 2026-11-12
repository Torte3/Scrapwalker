using System;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Save
{
    public readonly struct SaveSlot
    {
        private SaveSlot(int index, bool exists, bool hasHeading, DateTime savedAt, string furthestRuin, int ticks, bool hasEscaped, TimeOfDay timeOfDay)
        {
            Index = index;
            HasSave = exists;
            HasHeading = hasHeading;
            SavedAt = savedAt;
            FurthestRuin = furthestRuin;
            Ticks = ticks;
            HasEscaped = hasEscaped;
            TimeOfDay = timeOfDay;
        }

        public int Index { get; }
        public bool HasSave { get; }
        public bool HasHeading { get; }
        public DateTime SavedAt { get; }
        public string FurthestRuin { get; }
        public int Ticks { get; }
        public bool HasEscaped { get; }
        public TimeOfDay TimeOfDay { get; }

        public bool IsAuto => Index == SaveStore.AutoSlot;

        public static SaveSlot Empty(int index)
        {
            return new SaveSlot(index, false, false, default, string.Empty, 0, false, TimeOfDay.Morning);
        }

        public static SaveSlot Unreadable(int index)
        {
            return new SaveSlot(index, true, false, default, string.Empty, 0, false, TimeOfDay.Morning);
        }

        public static SaveSlot Saved(int index, DateTime savedAt, string furthestRuin, int ticks, bool hasEscaped, TimeOfDay timeOfDay)
        {
            return new SaveSlot(index, true, true, savedAt, furthestRuin, ticks, hasEscaped, timeOfDay);
        }
    }
}

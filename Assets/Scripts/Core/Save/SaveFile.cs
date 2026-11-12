using System;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Save
{
    public sealed class SaveFile
    {
        public DateTime SavedAt;
        public WorldMemento World;

        internal static SaveFile Of(World world)
        {
            return new SaveFile { SavedAt = DateTime.UtcNow, World = world.CreateMemento() };
        }

        internal void MarkEscaped()
        {
            World.Progress.Escaped = true;
        }
    }
}

using Scrapwalker.Core.Root;
using Scrapwalker.Core.Travel;

namespace Scrapwalker.Core.Save
{
    public sealed class SaveCues
    {
        private readonly World _world;

        internal SaveCues(World world)
        {
            _world = world;
            world.Events.On<Departed>(departed =>
            {
                if (!departed.AfterDefeat)
                {
                    CueAutoSave();
                }
            });
            world.Events.On<Arriving>(_ => CueAutoSave());
            world.Events.On<EscapeChosen>(_ => CueEscape());
            world.Events.On<TakeoffFinished>(_ => MarkEscaped());
        }

        internal SaveFile AutoSave { get; private set; }

        internal SaveFile BeforeEscape { get; private set; }

        private SaveFile CueAutoSave()
        {
            if (AutoSave == null && _world.CanSave)
            {
                AutoSave = SaveFile.Of(_world);
            }

            return AutoSave;
        }

        private void CueEscape()
        {
            BeforeEscape = CueAutoSave() ?? SaveFile.Of(_world);
        }

        private void MarkEscaped()
        {
            BeforeEscape?.MarkEscaped();
        }

        internal SaveFile TakeAutoSave()
        {
            SaveFile cue = AutoSave;
            AutoSave = null;
            return cue;
        }
    }
}

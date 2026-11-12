using System.Linq;
using Scrapwalker.Core.Combat;
using Scrapwalker.Core.Combat.Waves;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Travel;

namespace Scrapwalker.Core.Root
{
    public sealed class Progress
    {
        private readonly bool[] _obtained = new bool[QualityRules.Count];

        internal Progress(World world)
        {
            world.Events.On<Looted>(looted =>
            {
                NoteObtained(looted.Items.Quality);
                MaterialsCollected += looted.Items.Count;
            });
            world.Events.On<MaterialsMade>(made => NoteObtained(made.Items.Quality));
            world.Events.On<TakeoffFinished>(_ => HasEscaped = true);
            world.Events.On<EnemyKilled>(_ => EnemiesKilled++);
            world.Events.On<WaveCleared>(_ => WavesHeld++);
        }

        public bool HasEscaped { get; internal set; }
        public bool HasBeenDefeated { get; internal set; }
        public int EnemiesKilled { get; private set; }
        public int MaterialsCollected { get; private set; }
        public int WavesHeld { get; private set; }

        public bool HasObtained(Quality quality)
        {
            return _obtained[(int)quality];
        }

        internal void NoteObtained(Quality quality)
        {
            _obtained[(int)quality] = true;
        }

        internal ProgressMemento CreateMemento()
        {
            return new ProgressMemento
            {
                Obtained = QualityRules.All.Where(HasObtained).ToList(),
                Escaped = HasEscaped,
                Defeated = HasBeenDefeated,
                EnemiesKilled = EnemiesKilled,
                MaterialsCollected = MaterialsCollected,
                WavesHeld = WavesHeld,
            };
        }

        internal void Restore(ProgressMemento memento)
        {
            System.Array.Clear(_obtained, 0, _obtained.Length);
            foreach (Quality quality in memento.Obtained)
            {
                NoteObtained(quality);
            }

            HasEscaped = memento.Escaped;
            HasBeenDefeated = memento.Defeated;
            EnemiesKilled = memento.EnemiesKilled;
            MaterialsCollected = memento.MaterialsCollected;
            WavesHeld = memento.WavesHeld;
        }
    }
}

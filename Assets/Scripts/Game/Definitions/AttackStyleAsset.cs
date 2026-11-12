using Scrapwalker.Core.Combat;
using Scrapwalker.Game.Audio.Sounds;
using Scrapwalker.Game.Look.Art;
using UnityEngine;

namespace Scrapwalker.Game.Definitions
{
    public sealed class AttackStyleAsset : DefinitionAsset<AttackStyleDefinition>
    {
        [SerializeField] private ArtCue _cue;
        [SerializeField] private MoveSounds _sounds;

        public ArtCue Cue => _cue;
        public MoveSounds Sounds => _sounds;
    }
}

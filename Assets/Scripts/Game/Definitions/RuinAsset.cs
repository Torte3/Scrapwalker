using Scrapwalker.Core.Travel;
using Scrapwalker.Game.Look.Art;
using UnityEngine;

namespace Scrapwalker.Game.Definitions
{
    [CreateAssetMenu(fileName = "Ruin", menuName = "Scrapwalker/Ruin")]
    public sealed class RuinAsset : DefinitionAsset<RuinDefinition>
    {
        [SerializeField] private string _name;
        [SerializeField] private AudioClip _music;
        [SerializeField] private ArtEntry _picture;
        [SerializeField] private ArtLayer _back;
        [SerializeField] private ArtLayer _front;

        public override string Name => _name;
        public AudioClip Music => _music;
        public ArtEntry Picture => _picture;
        public ArtLayer Back => _back;
        public ArtLayer Front => _front;
    }
}

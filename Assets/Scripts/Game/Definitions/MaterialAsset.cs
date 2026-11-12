using Scrapwalker.Core.Items;
using Scrapwalker.Game.Look.Art;
using UnityEngine;

namespace Scrapwalker.Game.Definitions
{
    [CreateAssetMenu(fileName = "Material", menuName = "Scrapwalker/Material")]
    public sealed class MaterialAsset : DefinitionAsset<MaterialDefinition>
    {
        [SerializeField] private string _name;
        [SerializeField] private ArtLayer _iconArt;
        [SerializeField] private ArtLayer _dropArt;
        [SerializeField] private int _textSprite;

        public override string Name => _name;
        public int TextSprite => _textSprite;
        public ArtLayer IconArt => _iconArt;
        public ArtLayer DropArt => _dropArt;
    }
}

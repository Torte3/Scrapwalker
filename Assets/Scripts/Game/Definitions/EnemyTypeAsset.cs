using System.Collections.Generic;
using Scrapwalker.Core.Combat;
using Scrapwalker.Game.Audio.Sounds;
using UnityEngine;

namespace Scrapwalker.Game.Definitions
{
    [CreateAssetMenu(fileName = "EnemyType", menuName = "Scrapwalker/Enemy Type")]
    public sealed class EnemyTypeAsset : DefinitionAsset<EnemyTypeDefinition>
    {
        [SerializeField] private string _name;
        [SerializeField] private List<AttackStyleAsset> _styles;
        [SerializeField] private SoundCue _killed;

        public override string Name => _name;
        public IReadOnlyList<AttackStyleAsset> Styles => _styles;
        public SoundCue Killed => _killed;
    }
}

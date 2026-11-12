using Scrapwalker.Core.Combat;
using UnityEngine;

namespace Scrapwalker.Game.Definitions
{
    [CreateAssetMenu(fileName = "Enemy", menuName = "Scrapwalker/Enemy")]
    public sealed class EnemyAsset : DefinitionAsset<EnemyDefinition>
    {
        [SerializeField] private EnemyArt _look = new EnemyArt();

        public EnemyArt Look => _look;
    }
}

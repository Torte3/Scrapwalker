using System.Collections.Generic;
using Scrapwalker.Core.Combat;
using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.Look.Art;

namespace Scrapwalker.Game.MainView.Enemies
{
    public sealed class EnemyLooks
    {
        private readonly GameData _data;
        private readonly Dictionary<EnemyDefinition, EnemyLook> _looks = new Dictionary<EnemyDefinition, EnemyLook>();

        public EnemyLooks(GameData data)
        {
            _data = data;
        }

        public EnemyLook Of(EnemyDefinition definition)
        {
            if (!_looks.TryGetValue(definition, out EnemyLook look))
            {
                look = new EnemyLook(definition, _data.Asset(definition).Look);
                _looks[definition] = look;
            }

            return look;
        }

        public ArtCue CueOf(AttackStyleDefinition style)
        {
            return style != null ? _data.Asset(style).Cue : ArtCue.Always;
        }
    }
}

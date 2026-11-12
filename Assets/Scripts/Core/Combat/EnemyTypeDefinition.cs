using System;
using System.Collections.Generic;
using Scrapwalker.Core.Framework;
using UnityEngine;

namespace Scrapwalker.Core.Combat
{
    [Serializable]
    public sealed class EnemyTypeDefinition : Definition
    {
        public float Speed = 1f;
        public float StopDistance;
        [SerializeReference] public List<EnemyAbility> Abilities = new List<EnemyAbility>();
    }
}

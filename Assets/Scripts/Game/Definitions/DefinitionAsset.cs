using Scrapwalker.Core.Framework;
using UnityEngine;

namespace Scrapwalker.Game.Definitions
{
    public abstract class DefinitionAsset : ScriptableObject
    {
        public const string DefinitionField = "_definition";

        public abstract Definition Definition { get; }

        public virtual string Name => name;
    }
}

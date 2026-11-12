using Scrapwalker.Core.Framework;
using UnityEngine;

namespace Scrapwalker.Game.Definitions
{
    public abstract class DefinitionAsset<T> : DefinitionAsset where T : Definition, new()
    {
        [SerializeField] private T _definition = new T();

        public override Definition Definition => _definition;

        public T Value => _definition;
    }
}

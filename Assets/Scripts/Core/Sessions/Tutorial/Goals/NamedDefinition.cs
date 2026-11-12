using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Sessions.Tutorial.Goals
{
    public readonly struct NamedDefinition<T> where T : Definition
    {
        public NamedDefinition(string key)
        {
            Key = key;
        }

        public string Key { get; }

        public T Of(World world)
        {
            return world.Definitions.Named<T>(Key);
        }

        public bool TryIn(GameDefinitions definitions, out T definition)
        {
            return definitions.TryNamed(Key, out definition);
        }
    }
}

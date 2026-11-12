using System;
using Scrapwalker.Core.Framework;

namespace Scrapwalker.Core.Items
{
    [Serializable]
    public sealed class MaterialDefinition : Definition
    {
        public int Stack = 1;

        public int StackLimit => Math.Max(1, Stack);
    }
}

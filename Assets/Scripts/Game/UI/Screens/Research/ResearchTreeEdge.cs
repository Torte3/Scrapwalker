using System.Collections.Generic;
using Scrapwalker.Core.Research;
using UnityEngine;

namespace Scrapwalker.Game.UI.Screens.Research
{
    public sealed class ResearchTreeEdge
    {
        public ResearchTreeEdge(ResearchDefinition from, ResearchDefinition to, IReadOnlyList<Vector2> points)
        {
            From = from;
            To = to;
            Points = points;
        }

        public ResearchDefinition From { get; }
        public ResearchDefinition To { get; }
        public IReadOnlyList<Vector2> Points { get; }
    }
}

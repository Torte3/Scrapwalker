using Scrapwalker.Core.Research;
using UnityEngine;

namespace Scrapwalker.Game.UI.Screens.Research
{
    public sealed class ResearchTreeNode
    {
        public ResearchTreeNode(ResearchDefinition definition, int column)
        {
            Definition = definition;
            Column = column;
            Position = new Vector2(column * ResearchTreeLayout.ColumnWidth, definition.Row * ResearchTreeLayout.RowHeight);
        }

        public ResearchDefinition Definition { get; }
        public int Column { get; }
        public Vector2 Position { get; }
    }
}

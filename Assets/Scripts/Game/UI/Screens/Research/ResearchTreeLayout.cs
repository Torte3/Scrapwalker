using System;
using System.Collections.Generic;
using System.Linq;
using Scrapwalker.Core.Research;
using Scrapwalker.Core.Root;
using UnityEngine;

namespace Scrapwalker.Game.UI.Screens.Research
{
    public sealed class ResearchTreeLayout
    {
        public const float Unit = 16f;
        public const float NodeWidth = 10.4f * Unit;
        public const float NodeHeight = 5f * Unit;
        public const float ColumnWidth = 15f * Unit;
        public const float RowHeight = 6f * Unit;
        private const float PortStep = NodeHeight * 0.14f;
        private const float HitMargin = 3f;

        private readonly List<ResearchTreeNode> _nodes = new List<ResearchTreeNode>();
        private readonly List<ResearchTreeEdge> _edges = new List<ResearchTreeEdge>();
        private readonly Dictionary<ResearchDefinition, ResearchTreeNode> _byDefinition = new Dictionary<ResearchDefinition, ResearchTreeNode>();
        private readonly Dictionary<int, List<ResearchTreeNode>> _columns = new Dictionary<int, List<ResearchTreeNode>>();
        private readonly Dictionary<Link, float> _outOffsets = new Dictionary<Link, float>();
        private readonly Dictionary<Link, float> _inOffsets = new Dictionary<Link, float>();

        private ResearchTreeLayout()
        {
        }

        public IReadOnlyList<ResearchTreeNode> Nodes => _nodes;
        public IReadOnlyList<ResearchTreeEdge> Edges => _edges;
        public Vector2 Size { get; private set; }

        public static ResearchTreeLayout Build(GameDefinitions definitions)
        {
            var layout = new ResearchTreeLayout();
            layout.Arrange(definitions);
            return layout;
        }

        private void Arrange(GameDefinitions definitions)
        {
            var depth = new Dictionary<ResearchDefinition, int>();
            foreach (ResearchDefinition definition in definitions.Research)
            {
                var node = new ResearchTreeNode(definition, Depth(definition, depth));
                _nodes.Add(node);
                _byDefinition.TryAdd(definition, node);
                if (!_columns.TryGetValue(node.Column, out List<ResearchTreeNode> column))
                {
                    column = new List<ResearchTreeNode>();
                    _columns[node.Column] = column;
                }

                column.Add(node);
            }

            Size = new Vector2(
                _nodes.Select(n => n.Position.x + NodeWidth).DefaultIfEmpty(0f).Max(),
                _nodes.Select(n => n.Position.y + NodeHeight).DefaultIfEmpty(0f).Max());
            foreach (List<ResearchTreeNode> column in _columns.Values)
            {
                column.Sort((p, q) => p.Position.y.CompareTo(q.Position.y));
            }

            List<Link> links = _nodes.SelectMany(node => node.Definition.Prerequisites.Select(from => new Link(from, node.Definition))).ToList();
            foreach (ResearchTreeNode node in _nodes)
            {
                SpreadPorts(node, links.Where(l => l.From == node.Definition).Select(l => (l, _byDefinition[l.To])), _outOffsets);
                SpreadPorts(node, links.Where(l => l.To == node.Definition).Select(l => (l, _byDefinition[l.From])), _inOffsets);
            }

            foreach (Link link in links)
            {
                _edges.Add(new ResearchTreeEdge(link.From, link.To, Route(link)));
            }
        }

        private static int Depth(ResearchDefinition definition, Dictionary<ResearchDefinition, int> knownDepths)
        {
            if (knownDepths.TryGetValue(definition, out int known))
            {
                return known;
            }

            int depth = definition.Prerequisites.Select(required => Depth(required, knownDepths) + 1).DefaultIfEmpty(0).Max();
            knownDepths[definition] = depth;
            return depth;
        }

        private static void SpreadPorts(ResearchTreeNode self, IEnumerable<(Link Link, ResearchTreeNode Other)> ends, Dictionary<Link, float> offsets)
        {
            var flat = new List<Link>();
            var up = new List<(Link Link, float Y)>();
            var down = new List<(Link Link, float Y)>();
            foreach ((Link link, ResearchTreeNode other) in ends)
            {
                float y = other.Position.y;
                if (Mathf.Approximately(y, self.Position.y))
                {
                    flat.Add(link);
                }
                else if (y < self.Position.y)
                {
                    up.Add((link, y));
                }
                else
                {
                    down.Add((link, y));
                }
            }

            foreach (Link link in flat)
            {
                offsets[link] = 0f;
            }

            int step = flat.Count > 0 || down.Count > 0 ? 1 : 0;
            foreach ((Link link, float _) in up.OrderByDescending(e => e.Y))
            {
                offsets[link] = -step++ * PortStep;
            }

            step = flat.Count > 0 || up.Count > 0 ? 1 : 0;
            foreach ((Link link, float _) in down.OrderBy(e => e.Y))
            {
                offsets[link] = step++ * PortStep;
            }
        }

        private List<Vector2> Route(Link link)
        {
            ResearchTreeNode a = _byDefinition[link.From];
            ResearchTreeNode b = _byDefinition[link.To];
            float y1 = a.Position.y + NodeHeight * 0.5f + _outOffsets[link];
            float y2 = b.Position.y + NodeHeight * 0.5f + _inOffsets[link];
            var points = new List<Vector2> { new Vector2(a.Position.x + NodeWidth, y1) };
            for (int c = a.Column + 1; c < b.Column; c++)
            {
                float t = (c - a.Column) / (float)(b.Column - a.Column);
                float straightY = y1 + (y2 - y1) * t;
                if (!CrossesNodeIn(c, straightY))
                {
                    continue;
                }

                float gap = GapNear(c, straightY);
                points.Add(new Vector2(c * ColumnWidth - 2f * Unit, gap));
                points.Add(new Vector2(c * ColumnWidth + NodeWidth + 2f * Unit, gap));
            }

            points.Add(new Vector2(b.Position.x, y2));
            return points;
        }

        private IReadOnlyList<ResearchTreeNode> ColumnNodes(int column)
        {
            return _columns.TryGetValue(column, out List<ResearchTreeNode> nodes) ? nodes : (IReadOnlyList<ResearchTreeNode>)Array.Empty<ResearchTreeNode>();
        }

        private bool CrossesNodeIn(int column, float y)
        {
            return ColumnNodes(column).Any(node => y > node.Position.y - HitMargin && y < node.Position.y + NodeHeight + HitMargin);
        }

        private float GapNear(int column, float want)
        {
            IReadOnlyList<ResearchTreeNode> nodes = ColumnNodes(column);
            var gaps = new List<float> { nodes[0].Position.y - RowHeight * 0.25f };
            for (int k = 0; k + 1 < nodes.Count; k++)
            {
                gaps.Add((nodes[k].Position.y + NodeHeight + nodes[k + 1].Position.y) * 0.5f);
            }

            gaps.Add(nodes[nodes.Count - 1].Position.y + NodeHeight + RowHeight * 0.25f);
            return gaps.OrderBy(g => Mathf.Abs(g - want)).First();
        }

        private readonly struct Link : IEquatable<Link>
        {
            public Link(ResearchDefinition from, ResearchDefinition to)
            {
                From = from;
                To = to;
            }

            public ResearchDefinition From { get; }
            public ResearchDefinition To { get; }

            public bool Equals(Link other)
            {
                return From == other.From && To == other.To;
            }

            public override bool Equals(object value)
            {
                return value is Link other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(From, To);
            }
        }
    }
}

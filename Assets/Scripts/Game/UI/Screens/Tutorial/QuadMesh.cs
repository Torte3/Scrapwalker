using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Screens.Tutorial
{
    internal sealed class QuadMesh
    {
        private readonly List<Rect> _rects = new List<Rect>();
        private readonly List<Color32> _colors = new List<Color32>();

        public void Clear()
        {
            _rects.Clear();
            _colors.Clear();
        }

        public void Add(Rect rect, Color32 color)
        {
            if (rect.width <= 0f || rect.height <= 0f || color.a == 0)
            {
                return;
            }

            _rects.Add(rect);
            _colors.Add(color);
        }

        public void Write(MeshGenerationContext context)
        {
            if (_rects.Count == 0)
            {
                return;
            }

            MeshWriteData mesh = context.Allocate(_rects.Count * 4, _rects.Count * 6);
            for (int i = 0; i < _rects.Count; i++)
            {
                Rect r = _rects[i];
                Color32 c = _colors[i];
                mesh.SetNextVertex(new Vertex { position = new Vector3(r.xMin, r.yMin, Vertex.nearZ), tint = c });
                mesh.SetNextVertex(new Vertex { position = new Vector3(r.xMax, r.yMin, Vertex.nearZ), tint = c });
                mesh.SetNextVertex(new Vertex { position = new Vector3(r.xMax, r.yMax, Vertex.nearZ), tint = c });
                mesh.SetNextVertex(new Vertex { position = new Vector3(r.xMin, r.yMax, Vertex.nearZ), tint = c });
                ushort start = (ushort)(i * 4);
                mesh.SetNextIndex(start);
                mesh.SetNextIndex((ushort)(start + 1));
                mesh.SetNextIndex((ushort)(start + 2));
                mesh.SetNextIndex((ushort)(start + 2));
                mesh.SetNextIndex((ushort)(start + 3));
                mesh.SetNextIndex(start);
            }
        }
    }
}

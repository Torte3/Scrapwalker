using Scrapwalker.Game.Look.Art.Parts;
using UnityEngine;

namespace Scrapwalker.Game.Look.Art
{
    public sealed class ArtEntry : ScriptableObject
    {
        [SerializeField] private Vector2 _origin;
        [SerializeField] private ArtLayer[] _layers;

        public Vector2 Origin => _origin;
        public ArtLayer[] Layers => _layers;

        public ArtLayer FirstLayer => _layers[0];

        public ArtFrame FirstFrame => FirstLayer.First;

        public T Find<T>() where T : ArtPart
        {
            foreach (ArtLayer layer in _layers)
            {
                T part = layer.Find<T>();
                if (part != null)
                {
                    return part;
                }
            }

            return null;
        }

#if UNITY_EDITOR
        public void Fill(Vector2 origin, ArtLayer[] layers)
        {
            _origin = origin;
            _layers = layers;
        }
#endif
    }
}

using System.Collections.Generic;
using UnityEngine;

namespace Scrapwalker.Game.MainView
{
    public sealed class RendererPool
    {
        private readonly List<SpriteRenderer> _renderers = new List<SpriteRenderer>();
        private readonly Transform _root;
        private readonly string _name;
        private int _used;

        public RendererPool(Transform root, string name)
        {
            _root = root;
            _name = name;
        }

        public void Begin()
        {
            _used = 0;
        }

        public SpriteRenderer Next()
        {
            if (_used == _renderers.Count)
            {
                var go = new GameObject(_name);
                go.transform.SetParent(_root, false);
                _renderers.Add(go.AddComponent<SpriteRenderer>());
            }

            SpriteRenderer renderer = _renderers[_used++];
            renderer.enabled = true;
            return renderer;
        }

        public void End()
        {
            for (int i = _used; i < _renderers.Count; i++)
            {
                if (_renderers[i].enabled)
                {
                    _renderers[i].enabled = false;
                }
            }
        }
    }
}

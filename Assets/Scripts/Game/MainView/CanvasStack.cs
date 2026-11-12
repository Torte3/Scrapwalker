using System.Collections.Generic;
using UnityEngine;

namespace Scrapwalker.Game.MainView
{
    public sealed class CanvasStack
    {
        private readonly Stack<State> _saved = new Stack<State>();
        private State _state = State.Fresh;

        private struct State
        {
            public static State Fresh => new State { Transform = CanvasTransform.Identity, Alpha = 1f };

            public CanvasTransform Transform;
            public float Alpha;
        }

        public float Alpha
        {
            get => _state.Alpha;
            set => _state.Alpha = value;
        }

        public CanvasTransform Transform => _state.Transform;

        public void Begin()
        {
            _saved.Clear();
            _state = State.Fresh;
        }

        public void Save()
        {
            _saved.Push(_state);
        }

        public void Restore()
        {
            if (_saved.Count > 0)
            {
                _state = _saved.Pop();
            }
        }

        public void Translate(float x, float y)
        {
            _state.Transform = _state.Transform.Translated(x, y);
        }

        public void Rotate(float angle)
        {
            _state.Transform = _state.Transform.Rotated(angle);
        }

        public void RotateAbout(Vector2 pivot, float angle)
        {
            Translate(pivot.x, pivot.y);
            Rotate(angle);
            Translate(-pivot.x, -pivot.y);
        }

        public void Scale(float sx, float sy)
        {
            _state.Transform = _state.Transform.Scaled(sx, sy);
        }

        public Vector2 ToCanvas(float x, float y)
        {
            return _state.Transform.Map(x, y);
        }

        public Vector2 FromCanvas(Vector2 p)
        {
            return _state.Transform.Unmap(p);
        }
    }
}

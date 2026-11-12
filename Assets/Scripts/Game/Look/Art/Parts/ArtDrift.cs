using System;
using UnityEngine;

namespace Scrapwalker.Game.Look.Art.Parts
{
    [Serializable]
    public sealed class ArtDrift : ArtPart
    {
        [SerializeField] private Vector2[] _bobs;
        [SerializeField] private ArtBump _bump;
        [SerializeField] private float _lungeReach;
        [SerializeField] private float _lungeOutEnd;
        [SerializeField] private float _lungeBackStart;

        public ArtDrift(Vector2[] bobs, ArtBump bump, float lungeReach, float lungeOutEnd, float lungeBackStart)
        {
            _bobs = bobs;
            _bump = bump;
            _lungeReach = lungeReach;
            _lungeOutEnd = lungeOutEnd;
            _lungeBackStart = lungeBackStart;
        }

        public float Bob(float t)
        {
            float y = 0f;
            foreach (Vector2 bob in _bobs)
            {
                y += Mathf.Sin(t * bob.y) * bob.x;
            }

            return y;
        }

        public override bool Place(IArtStage stage, ArtState state, ArtLayer layer)
        {
            float x = state.IsAttacking ? -Lunge(state.AttackCycle(layer)) : 0f;
            float y = Bob(state.Clock) - (state.IsWalking ? _bump.At(state.Clock) : 0f);
            stage.Translate(x, y);
            return true;
        }

        private float Lunge(float cycle)
        {
            if (_lungeReach == 0f)
            {
                return 0f;
            }

            if (cycle < _lungeOutEnd)
            {
                return cycle / _lungeOutEnd * _lungeReach;
            }

            if (cycle < _lungeBackStart)
            {
                return _lungeReach;
            }

            return _lungeReach * (1f - (cycle - _lungeBackStart) / (1f - _lungeBackStart));
        }
    }
}

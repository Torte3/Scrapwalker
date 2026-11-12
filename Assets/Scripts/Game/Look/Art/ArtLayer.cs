using Scrapwalker.Game.Look.Art.Parts;
using UnityEngine;

namespace Scrapwalker.Game.Look.Art
{
    public sealed class ArtLayer : ScriptableObject
    {
        [SerializeField] private string _part;
        [SerializeField] private ArtPlay _play;
        [SerializeField] private float _period;
        [SerializeField] private float _length;
        [SerializeField] private float[] _values;
        [SerializeField] private bool _isAdditive;
        [SerializeField] private ArtFrame[] _frames;
        [SerializeField] private ArtCondition _condition;
        [SerializeField] private ArtCue _cue;
        [SerializeField] private ArtLayer _replacement;
        [SerializeField] private ArtLayer _parent;
        [SerializeReference] private ArtPart[] _parts;

        public string Part => _part;
        public ArtPlay Play => _play;
        public float Period => _period;
        public float Length => _length;
        public float[] Values => _values;
        public bool IsAdditive => _isAdditive;
        public ArtFrame[] Frames => _frames;
        public ArtCondition Condition => _condition;
        public ArtCue Cue => _cue;
        public ArtLayer Replacement => _replacement;
        public ArtLayer Parent => _parent;
        public ArtPart[] Parts => _parts;

        public bool IsLoop => _play == ArtPlay.Loop;

        public ArtFrame First => _frames.Length > 0 ? _frames[0] : null;

        public T Find<T>() where T : ArtPart
        {
            foreach (ArtPart part in _parts)
            {
                if (part is T found)
                {
                    return found;
                }
            }

            return null;
        }

        public bool IsLight
        {
            get
            {
                foreach (ArtPart part in _parts)
                {
                    if (part.IsLight)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public float FirePhase => Find<ArtFire>()?.Phase ?? 0f;

        public float Alpha(float clock)
        {
            float alpha = 1f;
            foreach (ArtPart part in _parts)
            {
                alpha *= part.Alpha(clock);
            }

            return alpha;
        }

        public ArtFrame At(float t)
        {
            int n = _frames.Length;
            if (n == 0)
            {
                return null;
            }

            switch (_play)
            {
                case ArtPlay.Loop:
                    return _frames[Mathf.Min(n - 1, (int)(Mathf.Repeat(t, _period) / _period * n))];
                case ArtPlay.Trigger:
                    return _frames[Mathf.Clamp((int)(t / _length * n), 0, n - 1)];
                default:
                    return _frames[0];
            }
        }

        public ArtFrame FrameIn(ArtState state)
        {
            switch (_play)
            {
                case ArtPlay.Value:
                    return ByValue(state.ValueOf(this));
                case ArtPlay.Loop:
                    return At(state.Clock);
                default:
                    return First;
            }
        }

        public ArtFrame ByValue(float v)
        {
            int best = 0;
            for (int i = 1; i < _values.Length; i++)
            {
                if (Mathf.Abs(_values[i] - v) < Mathf.Abs(_values[best] - v))
                {
                    best = i;
                }
            }

            return best < _frames.Length ? _frames[best] : null;
        }

#if UNITY_EDITOR
        public void Fill(string part, ArtPlay play, float period, float length, float[] values, bool isAdditive, ArtFrame[] frames, ArtCondition condition, ArtCue cue, ArtPart[] parts)
        {
            _part = part;
            _play = play;
            _period = period;
            _length = length;
            _values = values;
            _isAdditive = isAdditive;
            _frames = frames;
            _condition = condition;
            _cue = cue;
            _parts = parts;
        }

        public void Link(ArtLayer replacement, ArtLayer parent)
        {
            _replacement = replacement;
            _parent = parent;
        }
#endif
    }
}

using Scrapwalker.Core.Root;
using UnityEngine;

namespace Scrapwalker.Game.MainView.Backdrop
{
    public sealed class SkyMix
    {
        public const float PlaySeconds = 10f;
        public const float LoadSeconds = 4f;

        private readonly float[] _from = new float[TimeOfDays.Count];
        private readonly float[] _weights = new float[TimeOfDays.Count];
        private float _elapsed;
        private float _seconds;

        public SkyMix()
        {
            Snap(TimeOfDay.Morning);
        }

        public TimeOfDay Target { get; private set; }

        public bool IsMoving => _elapsed < _seconds;

        public float Weight(TimeOfDay time)
        {
            return _weights[(int)time];
        }

        public void Snap(TimeOfDay time)
        {
            Target = time;
            for (int i = 0; i < _weights.Length; i++)
            {
                _weights[i] = i == (int)time ? 1f : 0f;
            }

            _elapsed = 0f;
            _seconds = 0f;
        }

        public void MoveTo(TimeOfDay time, float seconds)
        {
            if (time == Target)
            {
                return;
            }

            Target = time;
            System.Array.Copy(_weights, _from, _weights.Length);
            _elapsed = 0f;
            _seconds = seconds;
            if (seconds <= 0f)
            {
                Snap(time);
            }
        }

        public void Step(float deltaTime)
        {
            if (!IsMoving)
            {
                return;
            }

            _elapsed = Mathf.Min(_seconds, _elapsed + deltaTime);
            float u = Mathf.SmoothStep(0f, 1f, _elapsed / _seconds);
            for (int i = 0; i < _weights.Length; i++)
            {
                _weights[i] = Mathf.Lerp(_from[i], i == (int)Target ? 1f : 0f, u);
            }
        }

        public Color Mix(System.Func<TimeOfDay, Color> colorOf)
        {
            Color sum = Color.clear;
            for (int i = 0; i < _weights.Length; i++)
            {
                if (_weights[i] > 0f)
                {
                    sum += colorOf((TimeOfDay)i) * _weights[i];
                }
            }

            return sum;
        }

        public float Mix(System.Func<TimeOfDay, float> valueOf)
        {
            float sum = 0f;
            for (int i = 0; i < _weights.Length; i++)
            {
                if (_weights[i] > 0f)
                {
                    sum += valueOf((TimeOfDay)i) * _weights[i];
                }
            }

            return sum;
        }

        public float LayerAlpha(TimeOfDay time)
        {
            float before = 0f;
            for (int i = 0; i < (int)time; i++)
            {
                before += _weights[i];
            }

            float w = _weights[(int)time];
            return w <= 0f ? 0f : w / (before + w);
        }
    }
}

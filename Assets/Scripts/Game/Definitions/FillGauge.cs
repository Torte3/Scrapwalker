using System;
using UnityEngine;

namespace Scrapwalker.Game.Definitions
{
    [Serializable]
    public sealed class FillGauge : FacilityGauge
    {
        [SerializeField] private int _steps;
        [SerializeField] private bool _roundsUp;

        public override float Step(int stored, float fill, bool isActing)
        {
            if (!isActing)
            {
                return 0f;
            }

            float steps = Mathf.Clamp01(fill) * _steps;
            return _roundsUp ? Mathf.CeilToInt(steps - 0.01f) : Mathf.RoundToInt(steps);
        }
    }
}

using System;
using UnityEngine;

namespace Scrapwalker.Game.Look.Art.Parts
{
    [Serializable]
    public sealed class ArtSlide : ArtPart
    {
        private const float SlideBack = 0.15f;

        [SerializeField] private float _distance;

        public ArtSlide(float distance)
        {
            _distance = distance;
        }

        public override bool Place(IArtStage stage, ArtState state, ArtLayer layer)
        {
            float slide = state.IsAttacking ? _distance * Mathf.Max(0f, 1f - state.AttackCycle(layer) / SlideBack) : 0f;
            stage.Translate(slide, 0f);
            return true;
        }
    }
}

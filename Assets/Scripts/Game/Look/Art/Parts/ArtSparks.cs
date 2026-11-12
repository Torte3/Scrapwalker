using System;
using Scrapwalker.Game.Definitions;
using UnityEngine;

namespace Scrapwalker.Game.Look.Art.Parts
{
    [Serializable]
    public sealed class ArtSparks : ArtPart
    {
        [SerializeField] private Vector2 _at;
        [SerializeField] private int _everyFrames;
        [SerializeField] private SparkBurst _burst;

        public ArtSparks(Vector2 at, int everyFrames, SparkBurst burst)
        {
            _at = at;
            _everyFrames = everyFrames;
            _burst = burst;
        }

        public override void Emit(IArtStage stage)
        {
            if (stage.IsEvery(_everyFrames))
            {
                stage.Emit(_burst, _at);
            }
        }
    }
}

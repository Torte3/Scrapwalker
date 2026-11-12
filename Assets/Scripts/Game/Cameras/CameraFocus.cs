using UnityEngine;

namespace Scrapwalker.Game.Cameras
{
    public readonly struct CameraFocus
    {
        public CameraFocus(Vector2 facilityPixels, float facilityHeightPixels, float facilityWeight, float enemiesPixels, float enemiesWeight)
        {
            FacilityPixels = facilityPixels;
            FacilityHeightPixels = facilityHeightPixels;
            FacilityWeight = facilityWeight;
            EnemiesPixels = enemiesPixels;
            EnemiesWeight = enemiesWeight;
        }

        public Vector2 FacilityPixels { get; }
        public float FacilityHeightPixels { get; }
        public float FacilityWeight { get; }
        public float EnemiesPixels { get; }
        public float EnemiesWeight { get; }
    }
}

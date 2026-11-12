using System;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost;
using UnityEngine;

namespace Scrapwalker.Game.Definitions
{
    [Serializable]
    public sealed class StoredGauge : FacilityGauge
    {
        [SerializeField] private MaterialAsset _material;

        public override int CountStored(Facility facility)
        {
            return facility.Inventory != null ? facility.Inventory.CountOf(_material.Value) : 0;
        }

        public override float Step(int stored, float fill, bool isActing)
        {
            if (stored <= 0)
            {
                return 0f;
            }

            return stored < 10 ? 1f : 10f;
        }
    }
}

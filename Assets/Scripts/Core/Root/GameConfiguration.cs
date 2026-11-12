using System.Collections.Generic;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost.Upgrades;
using Scrapwalker.Core.Travel.Talk;
using UnityEngine;

namespace Scrapwalker.Core.Root
{
    [System.Serializable]
    public sealed class GameConfiguration
    {
        public List<StartFacility> StartFacilities = new List<StartFacility>();

        public float TickSeconds;
        public int GridColumns;
        public int GridRows;
        public float BuildSeconds;
        public float RelocateSeconds;
        public float DismantleSeconds;

        public float OutputQualityDownChance;
        public List<QualityRow> Qualities = new List<QualityRow>();

        public Quality StartQuality;
        public List<MaterialAmount> StartMaterials = new List<MaterialAmount>();
        public float BatterySeconds;
        public float CoalEnergy;
        public float EnergyCapacity;
        public float ChargeThreshold;
        public float LowBatterySpeed;
        public float ChargeSeconds;

        public int FloorSlotsPerCell;

        public int HullMax;
        public int HullCracks;
        public List<MaterialAmount> RepairCost = new List<MaterialAmount>();
        public float RepairSeconds;
        public float SpawnDistance;
        public float CellDistance;
        public float ArmorDistance;
        public float HaulSeconds;

        public float LandToDigSeconds;
        public float ArmReachSeconds;
        public float WaveGapSeconds;

        public float BarrierEnergyPerPoint;
        public float WeaponRange;

        public int FacilityHP;
        public float FacilityHPRegeneration;

        public float BotWalkSpeed;
        public float IdleWalkSpeed;
        public float IdleWanderCells;
        public float IdleRestMinSeconds;
        public float IdleRestMaxSeconds;

        public int StartBots;
        [SerializeReference] public List<UpgradeEffect> StartEffects = new List<UpgradeEffect>();

        public float BlackoutFadeSeconds;
        public float DefeatDarkenSeconds;
        public float DefeatHoldSeconds;
        public float DefeatBrightenSeconds;
        public float TakeoffSeconds;

        public float TalkCharsPerSecond;
        public List<CuedTalk> Talks = new List<CuedTalk>();

        public float AutoSaveSeconds;

        public float TravelSeconds;
        public float TravelClearSeconds;

        public QualityRow Row(Quality quality)
        {
            return Qualities[(int)quality];
        }

        public int TicksFor(float seconds)
        {
            return (int)System.Math.Round(seconds / TickSeconds, System.MidpointRounding.AwayFromZero);
        }

        public int TicksAtLeastOne(float seconds)
        {
            return System.Math.Max(1, TicksFor(seconds));
        }
    }
}

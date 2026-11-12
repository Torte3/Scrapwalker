using System;
using System.Collections.Generic;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Game.MainView.Compose
{
    public sealed class FrameVisual
    {
        private bool[] _occupied = Array.Empty<bool>();

        public World World { get; private set; }
        public List<FacilityVisual> Facilities { get; } = new List<FacilityVisual>();
        public List<BotVisual> Bots { get; } = new List<BotVisual>();
        public int RadarLook { get; set; }
        public float Wake { get; set; }
        public TravelVisual Travel { get; set; }
        public ArmVisual Arm { get; set; }
        public LaunchVisual Launch { get; set; }
        public OverlayVisual Overlay { get; set; }

        public void Begin(World world)
        {
            World = world;
            Facilities.Clear();
            Bots.Clear();
            int cells = world.Columns * world.Rows;
            if (_occupied.Length != cells)
            {
                _occupied = new bool[cells];
            }
            else
            {
                Array.Clear(_occupied, 0, _occupied.Length);
            }
        }

        public void Occupy(FacilityRect rect)
        {
            for (int row = Math.Max(0, rect.Row); row < Math.Min(World.Rows, rect.Row + rect.Height); row++)
            {
                for (int column = Math.Max(0, rect.Column); column < Math.Min(World.Columns, rect.Column + rect.Width); column++)
                {
                    _occupied[row * World.Columns + column] = true;
                }
            }
        }

        public bool IsOccupied(int column, int row)
        {
            return _occupied[row * World.Columns + column];
        }

        public bool TryGetFacility(int facilityID, out FacilityVisual facility)
        {
            foreach (FacilityVisual candidate in Facilities)
            {
                if (candidate.Facility.ID == facilityID)
                {
                    facility = candidate;
                    return true;
                }
            }

            facility = default;
            return false;
        }
    }
}

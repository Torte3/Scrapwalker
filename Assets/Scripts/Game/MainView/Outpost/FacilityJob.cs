using Scrapwalker.Core.Outpost;
using Scrapwalker.Game.Look;
using Scrapwalker.Game.MainView.Compose;
using Scrapwalker.Game.Wording.Words;
using UnityEngine;

namespace Scrapwalker.Game.MainView.Outpost
{
    public abstract class FacilityJob
    {
        public static readonly FacilityJob None = new NoJob();
        public static readonly FacilityJob Building = new BuildingJob();
        public static readonly FacilityJob Rebuilding = new RebuildingJob();
        public static readonly FacilityJob Reassembling = new SiteJob(WorldWords.Reassembling);
        public static readonly FacilityJob Upgrading = new UpgradingJob();
        public static readonly FacilityJob Dismantling = new DismantlingJob();

        private FacilityJob()
        {
        }

        public virtual bool IsUnderWay => true;
        public virtual bool HasPicture => true;
        public virtual Color NameColor => Palette.FacilityName;
        public abstract string Word { get; }
        public virtual Color TextColor => Palette.Amber;
        public virtual Color? BarColor => null;

        public virtual float ProgressOf(Facility facility)
        {
            return facility.Site.Progress;
        }

        public virtual bool IsManned(in FacilityVisual facility)
        {
            return facility.Facility.IsStaffed;
        }

        internal virtual void Draw(FacilityPainter painter, in FacilityVisual facility, float recoil)
        {
            painter.DrawStopped(facility, recoil);
            painter.DrawBuildStripes(facility, Color.white);
        }

        internal virtual void Emit(FacilityPainter painter, in FacilityVisual facility)
        {
            if (facility.Facility.IsStaffed)
            {
                painter.EmitJobSparks(facility);
            }
        }

        private sealed class NoJob : FacilityJob
        {
            public override bool IsUnderWay => false;
            public override string Word => WorldWords.Building;

            internal override void Draw(FacilityPainter painter, in FacilityVisual facility, float recoil)
            {
                painter.DrawWorking(facility, recoil);
            }

            internal override void Emit(FacilityPainter painter, in FacilityVisual facility)
            {
            }
        }

        private class SiteJob : FacilityJob
        {
            private readonly string _word;

            public SiteJob(string word)
            {
                _word = word;
            }

            public override string Word => _word;
        }

        private sealed class BuildingJob : FacilityJob
        {
            public override bool HasPicture => false;
            public override string Word => WorldWords.Building;

            internal override void Draw(FacilityPainter painter, in FacilityVisual facility, float recoil)
            {
                painter.DrawBuildStripes(facility, Color.white);
            }
        }

        private sealed class RebuildingJob : FacilityJob
        {
            public override Color NameColor => Palette.Repairing;
            public override string Word => WorldWords.Rebuilding;
            public override Color TextColor => Palette.Repairing;

            internal override void Draw(FacilityPainter painter, in FacilityVisual facility, float recoil)
            {
                painter.DrawStopped(facility, recoil);
                painter.DrawDamage(facility);
                painter.DrawBuildStripes(facility, Palette.OverPicture);
            }

            internal override void Emit(FacilityPainter painter, in FacilityVisual facility)
            {
                base.Emit(painter, facility);
                painter.EmitRepairSparks(facility);
            }
        }

        private sealed class UpgradingJob : FacilityJob
        {
            public override string Word => WorldWords.Upgrading;

            public override float ProgressOf(Facility facility)
            {
                return facility.Work.WorkProgress;
            }

            public override bool IsManned(in FacilityVisual facility)
            {
                return facility.IsWorking;
            }
        }

        private sealed class DismantlingJob : FacilityJob
        {
            public override string Word => WorldWords.Dismantling;
            public override Color TextColor => Palette.Dismantling;
            public override Color? BarColor => Palette.DismantleBar;

            internal override void Draw(FacilityPainter painter, in FacilityVisual facility, float recoil)
            {
                painter.DrawStopped(facility, recoil);
                painter.DrawDismantleShade(facility);
            }

            internal override void Emit(FacilityPainter painter, in FacilityVisual facility)
            {
                if (facility.Facility.IsStaffed)
                {
                    painter.EmitDismantleDust(facility);
                }
            }
        }
    }
}
